using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class BasicAPIGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.BasicAPI";

    private const string Suffix = "API";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<INamedTypeSymbol?> provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0,
                static (ctx, _) => ClassHelper.GetWithAttribute(ctx, Attribute)
            )
            .Where(static c => c is not null);

        context.RegisterSourceOutput(
            provider,
            static (spc, source) =>
                GeneratorHelper.Execute(spc, source!, Suffix, GeneratePartialClass)
        );
    }

    private static string GeneratePartialClass(INamedTypeSymbol symbol)
    {
        StringBuilder sb = new();

        sb.AppendLine("#nullable enable");

        string namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : $"namespace {symbol.ContainingNamespace.ToDisplayString()}\n{{\n";

        if (!string.IsNullOrEmpty(namespaceName))
            sb.Append(namespaceName);

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();

        sb.AppendLine($"    {accessibility} static partial class {symbol.Name}");
        sb.AppendLine("    {");
        sb.AppendLine("        private static readonly Ethereal.Classes.API.BaseAPI API = new();");

        if (!symbol.GetMembers("SetReady").OfType<IMethodSymbol>().Any())
        {
            sb.AppendLine();
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Mark the API as ready.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        internal static void SetReady() => API.SetReady();");
        }

        sb.AppendLine();
        sb.AppendLine(
            "        internal static async System.Threading.Tasks.Task<bool> WhenReady() => await API.WhenReady();"
        );

        sb.AppendLine("    }");

        if (namespaceName is not null)
            sb.AppendLine("}");

        return sb.ToString();
    }
}
