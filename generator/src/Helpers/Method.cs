using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators.Helpers;

internal static class MethodHelper
{
    internal static MethodMetadata? GetWithAttribute(
        GeneratorSyntaxContext context,
        string attributeName
    )
    {
        MethodDeclarationSyntax declaration = (MethodDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not IMethodSymbol symbol)
            return null;

        INamedTypeSymbol? attribute = context.SemanticModel.Compilation.GetTypeByMetadataName(
            attributeName
        );

        if (attribute == null)
            return null;

        return symbol
            .GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, attribute))
            ? new MethodMetadata(
                symbol,
                GetParameters(symbol),
                GetMethodComments(declaration),
                null
            )
            : null;
    }

    internal static ImmutableArray<Member> GetParameters(IMethodSymbol symbol) =>
        [
            .. symbol.Parameters.Select(x => new Member(
                x.Type.ToDisplayString(),
                x.Name,
                x.Type.NullableAnnotation == NullableAnnotation.Annotated ? "null"
                    : x.HasExplicitDefaultValue
                        ? MemberHelper.ParseDefaultValue(x.ExplicitDefaultValue ?? "null")
                    : null
            )),
        ];

    internal static List<string> GetMethodComments(MethodDeclarationSyntax declaration) =>
        [
            .. declaration
                .GetLeadingTrivia()
                .Where(t =>
                    t.IsKind(SyntaxKind.SingleLineCommentTrivia)
                    || t.IsKind(SyntaxKind.MultiLineCommentTrivia)
                    || t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                )
                .Select(t => t.ToString()),
        ];
}
