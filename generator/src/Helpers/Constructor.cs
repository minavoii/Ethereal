using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators.Helpers;

internal static class ConstructorHelper
{
    internal static ConstructorMetadata? GetWithAttributeAndProperty(
        GeneratorSyntaxContext context,
        string attributeName,
        string propertyName
    )
    {
        var declaration = (ConstructorDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not IMethodSymbol symbol)
            return null;

        var attribute = context.SemanticModel.Compilation.GetTypeByMetadataName(attributeName);

        if (attribute == null)
            return null;

        return
            symbol
                .GetAttributes()
                .FirstOrDefault(x =>
                    SymbolEqualityComparer.Default.Equals(x.AttributeClass, attribute)
                )
                is AttributeData attributeData
            && attributeData
                .AttributeClass?.GetMembers()
                .FirstOrDefault(x => x.Name == propertyName)
                is IPropertySymbol property
            ? new ConstructorMetadata(
                symbol,
                MethodHelper.GetParameters(symbol),
                GetArguments(symbol, context),
                null,
                AttributeHelper.GetProperties(property)
            )
            : null;
    }

    internal static ImmutableArray<string> GetArguments(
        IMethodSymbol symbol,
        GeneratorSyntaxContext context
    )
    {
        var syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault(x =>
            x.GetSyntax() is ConstructorDeclarationSyntax
        );

        if (syntaxReference == null)
            return [];

        var declaration = (ConstructorDeclarationSyntax)syntaxReference.GetSyntax();
        var initializer = declaration.Initializer;

        if (initializer?.Kind() != SyntaxKind.ThisConstructorInitializer)
            return [];

        var args = initializer.ArgumentList.Arguments.Select(x =>
            x.Expression is ObjectCreationExpressionSyntax objectCreation
            && context.SemanticModel.GetTypeInfo(objectCreation).Type is ITypeSymbol objectType
                ? objectType.ToDisplayString()
                : x.Expression.ToString()
        );

        return [.. args];
    }
}
