using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class TryGetMethodGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.TryGet";

    private const string Suffix = "TryGet";

    private const string AdditionalComments =
        "/// <param name=\"result\"></param>\n"
        + "/// <returns>true if the API is ready and it was found; otherwise, false.</returns>";

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
            IMethodSymbol method = data.Symbol!;
            string name = method.Name.Replace("Get", "TryGet");
            string args = string.Join(", ", method.Parameters.Select(p => p.Name));
            string @params = string.Join(", ", data.Parameters?.Select(x => x.AsArgument()));
            string paramSeparator = @params == string.Empty ? "" : ", ";
            string returnType = method.ReturnType.ToString();

            if (returnType.EndsWith("?"))
                returnType = returnType.Remove(returnType.Length - 1);

            sb.AppendLine($"        /// {string.Join("\n", data.Comments)}");
            sb.AppendLine(AdditionalComments);
            sb.AppendLine(
                $"        public static bool {name}({@params}{paramSeparator}out {returnType} result)"
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
