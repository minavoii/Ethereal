using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class GetObjectGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.GetObject";

    private const string Suffix = "GetObject";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<MethodMetadata> provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0,
                static (ctx, _) => MethodHelper.GetWithAttribute(ctx, Attribute)!
            )
            .Where(static m => m is not null);

        IncrementalValueProvider<(
            Compilation Left,
            ImmutableArray<MethodMetadata> Right
        )> compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(
            compilation,
            static (spc, source) =>
                GeneratorHelper.ExecuteGroup<MethodMetadata, IMethodSymbol>(
                    spc,
                    source.Right!,
                    Suffix,
                    GeneratePartialClass
                )
        );
    }

    private static string GeneratePartialClass(ISymbol symbol, IEnumerable<MethodMetadata> methods)
    {
        StringBuilder sb = new();

        sb.AppendLine("#nullable enable");

        string @namespace = symbol.ContainingNamespace.ToDisplayString();

        if (!string.IsNullOrEmpty(@namespace))
        {
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
        }

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();
        sb.AppendLine($"    {accessibility} static partial class {symbol.Name}");
        sb.AppendLine("    {");

        foreach (MethodMetadata data in methods)
        {
            IMethodSymbol method = data.Symbol;
            string args = string.Join(", ", method.Parameters.Select(p => p.Name));
            string @params = string.Join(", ", data.Parameters?.Select(x => x.AsArgument()));

            if (data.Comments is List<string> comments)
                sb.AppendLine(CommentHelper.ToString(comments));

            sb.AppendLine(
                $"        public static async System.Threading.Tasks.Task<UnityEngine.GameObject?> GetObject({@params}) =>"
            );
            sb.AppendLine($"            (await {method.Name}({args}))?.gameObject;");
        }

        // Class ends
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(@namespace))
            sb.AppendLine("}");

        return sb.ToString();
    }
}
