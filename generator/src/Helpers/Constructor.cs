using System.Collections.Generic;
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
        ConstructorDeclarationSyntax declaration = (ConstructorDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not IMethodSymbol symbol)
            return null;

        INamedTypeSymbol? attribute = context.SemanticModel.Compilation.GetTypeByMetadataName(
            attributeName
        );

        if (attribute is null)
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
        SyntaxReference? syntaxReference = symbol.DeclaringSyntaxReferences.FirstOrDefault(x =>
            x.GetSyntax() is ConstructorDeclarationSyntax
        );

        if (syntaxReference is null)
            return [];

        ConstructorDeclarationSyntax declaration = (ConstructorDeclarationSyntax)
            syntaxReference.GetSyntax();
        ConstructorInitializerSyntax? initializer = declaration.Initializer;

        if (initializer?.Kind() != SyntaxKind.ThisConstructorInitializer)
            return [];

        IEnumerable<string> args = initializer.ArgumentList.Arguments.Select(x =>
            x.Expression is ObjectCreationExpressionSyntax objectCreation
            && context.SemanticModel.GetTypeInfo(objectCreation).Type is ITypeSymbol objectType
                ? objectType.ToDisplayString()
                : x.Expression.ToString()
        );

        return [.. args];
    }
}
