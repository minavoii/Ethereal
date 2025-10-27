using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators.Helpers;

internal static class RecordHelper
{
    internal static NamedTypeMetadata? GetWithAttribute(
        GeneratorSyntaxContext context,
        string attributeName
    )
    {
        var declaration = (RecordDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol)
            return null;

        var attribute = context.SemanticModel.Compilation.GetTypeByMetadataName(attributeName);

        if (attribute == null)
            return null;

        return symbol
            .GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attribute))
            ? new NamedTypeMetadata(
                symbol,
                [
                    .. symbol
                        .GetMembers()
                        .OfType<IPropertySymbol>()
                        .Where(x => !x.IsImplicitlyDeclared)
                        .Select(x => new Member(
                            x.Type.ToDisplayString(),
                            x.Name,
                            x.Type.NullableAnnotation == NullableAnnotation.Annotated ? "null"
                                : (
                                    (PropertyDeclarationSyntax)
                                        x.DeclaringSyntaxReferences[0].GetSyntax()
                                ).Initializer
                                    is EqualsValueClauseSyntax initializer
                                && context.SemanticModel.GetConstantValue(initializer.Value)
                                    is Optional<object> constant
                                && constant.HasValue
                                    ? MemberHelper.ParseDefaultValue(constant.Value!)
                                : null
                        )),
                ]
            )
            : null;
    }
}
