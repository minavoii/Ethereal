using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators.Helpers;

internal static class ClassHelper
{
    internal static INamedTypeSymbol? GetWithAttribute(
        GeneratorSyntaxContext context,
        string attributeName
    )
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol)
            return null;

        var attribute = context.SemanticModel.Compilation.GetTypeByMetadataName(attributeName);

        if (attribute == null)
            return null;

        return symbol
            .GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attribute))
            ? symbol
            : null;
    }
}
