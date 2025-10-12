using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Generators;

#nullable enable
[Generator]
public sealed class TryGetMethodGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Generator.TryGet";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0,
                static (ctx, _) => GetMethodWithAttribute(ctx)
            )
            .Where(static m => m is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(
            compilation,
            static (spc, source) => Execute(spc, source.Right!)
        );
    }

    private static SymbolWithComments? GetMethodWithAttribute(GeneratorSyntaxContext context)
    {
        var declaration = (MethodDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not IMethodSymbol symbol)
            return null;

        var serializableAttr = context.SemanticModel.Compilation.GetTypeByMetadataName(Attribute);

        if (serializableAttr == null)
            return null;

        return symbol
            .GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, serializableAttr))
            ? new SymbolWithComments() { Comments = GetComments(declaration), Symbol = symbol }
            : null;
    }

    private static string GetComments(MethodDeclarationSyntax declaration)
    {
        SyntaxTriviaList leadingTrivia = declaration.GetLeadingTrivia();

        var comments = leadingTrivia
            .Where(t =>
                t.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineCommentTrivia)
                || t.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.MultiLineCommentTrivia)
                || t.IsKind(
                    Microsoft.CodeAnalysis.CSharp.SyntaxKind.SingleLineDocumentationCommentTrivia
                )
            )
            .Select(x =>
            {
                var res = x.ToString();

                // Transform the <return> property into a <param> of name `result`
                if (res.Contains("<returns>"))
                    return res.Replace("<returns>", "<param name=\"result\">")
                        .Replace("</returns>", "</param>");
                else
                    return res;
            })
            .Where(x => x != String.Empty)
            .Concat(
                [
                    "/// <returns>true if the API is ready and it was found; otherwise, false.</returns>",
                ]
            );

        return string.Join("\n", comments.Select(t => t.ToString())) ?? String.Empty;
    }

    private static void Execute(
        SourceProductionContext context,
        ImmutableArray<SymbolWithComments> methods
    )
    {
        if (methods.IsDefaultOrEmpty)
            return;

        // Emit a single partial class
        var classes = methods.GroupBy(
            m => m.Symbol!.ContainingType,
            SymbolEqualityComparer.Default
        );

        foreach (var partialClass in classes)
        {
            ISymbol type = partialClass.Key!;

            context.AddSource(
                $"{type.Name}_TryGet.g.cs",
                SourceText.From(GeneratePartialClass(type, partialClass), Encoding.UTF8)
            );
        }
    }

    private static string GeneratePartialClass(
        ISymbol symbol,
        IEnumerable<SymbolWithComments> methods
    )
    {
        StringBuilder sb = new();

        string @namespace = symbol.ContainingNamespace.ToDisplayString();

        if (!string.IsNullOrEmpty(@namespace))
        {
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
        }

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();
        sb.AppendLine($"    {accessibility} static partial class {symbol.Name}");
        sb.AppendLine("    {");

        foreach (SymbolWithComments data in methods)
        {
            var method = data.Symbol!;
            var name = method.Name.Replace("Get", "TryGet");
            var args = string.Join(", ", method.Parameters.Select(p => p.Name));
            var @params = string.Join(
                ", ",
                method.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}")
            );
            var param_separator = @params == String.Empty ? "" : ", ";

            sb.AppendLine($"        /// {data.Comments}");
            sb.AppendLine(
                $"        public static bool {name}({@params}{param_separator}out {method.ReturnType} result)"
            );
            sb.AppendLine("        {");
            sb.AppendLine($"            result = API.IsReady ? {method.Name}({args}) : null;");
            sb.AppendLine("            return result != null;");
            sb.AppendLine("        }");
        }

        // Class ends
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(@namespace))
            sb.AppendLine("}");

        return sb.ToString();
    }
}

#nullable disable
