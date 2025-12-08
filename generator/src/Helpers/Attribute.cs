using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators.Helpers;

internal static class AttributeHelper
{
    internal static ImmutableArray<Member> GetProperties(IPropertySymbol property)
    {
        if (property.DeclaringSyntaxReferences.FirstOrDefault() is not SyntaxReference syntaxRef)
            return [];

        PropertyDeclarationSyntax declarationProp = (PropertyDeclarationSyntax)
            syntaxRef.GetSyntax();

        if (declarationProp.ExpressionBody?.Expression is not CollectionExpressionSyntax expression)
            return [];

        ImmutableArray<Member> tuples =
        [
            .. expression
                .Elements.Select(x =>
                    ((ExpressionElementSyntax)x).Expression is TupleExpressionSyntax tuple
                    && tuple.Arguments[0].Expression is TypeOfExpressionSyntax typeExpr
                    && tuple.Arguments[1].Expression is LiteralExpressionSyntax nameExpr
                        ? new Member(typeExpr.Type.ToString(), nameExpr.Token.ValueText)
                        : null! // Never included in the final result thanks to `Where` below
                )
                .Where(x => x is not null),
        ];

        return tuples;
    }
}
