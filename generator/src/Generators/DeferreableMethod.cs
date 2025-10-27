using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class DeferrableMethodGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.Deferrable";

    private const string Suffix = "Deferrable";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax m && m.AttributeLists.Count > 0,
                static (ctx, _) => MethodHelper.GetWithAttribute(ctx, Attribute)
            )
            .Where(static m => m is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

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
            var method = data.Symbol!;
            var name = method.Name.Replace("_Impl", "");
            var args = string.Join(", ", data.Parameters?.Select(p => p.Name));
            var @params = string.Join(", ", data.Parameters?.Select(x => x.AsArgument()));

            sb.AppendLine($"        /// {string.Join("\n", data.Comments)}");
            sb.AppendLine($"        public static void {name}({@params})");
            sb.AppendLine("        {");
            sb.AppendLine("            if (!API.IsReady)");
            sb.AppendLine($"                API.Enqueue(() => {name}({args}));");
            sb.AppendLine("           else");
            sb.AppendLine($"                {method.Name}({args});");
            sb.AppendLine("        }");
        }

        // Class ends
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(@namespace))
            sb.AppendLine("}");

        return sb.ToString();
    }
}
