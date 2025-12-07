using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators.Helpers;

internal static class ArgumentHelper
{
    internal static PropertyMetadata? GetWithAttribute(
        GeneratorSyntaxContext context,
        string attributeName
    )
    {
        PropertyDeclarationSyntax declaration = (PropertyDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not IPropertySymbol symbol)
            return null;

        INamedTypeSymbol? attribute = context.SemanticModel.Compilation.GetTypeByMetadataName(
            attributeName
        );

        if (attribute == null)
            return null;

        return
            symbol
                .GetAttributes()
                .FirstOrDefault(x =>
                    SymbolEqualityComparer.Default.Equals(x.AttributeClass, attribute)
                )
                is AttributeData attributeData
            ? new(
                symbol,
                [
                    .. attributeData.ConstructorArguments.Select(x => new Member(
                        x.Type?.ToString() ?? "",
                        x.Value?.ToString() ?? ""
                    )),
                ]
            )
            : null;
    }
}
