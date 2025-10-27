using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class DeferrableAPIGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.Deferrable";

    private const string Suffix = "API";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
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
        var sb = new StringBuilder();

        sb.AppendLine("#nullable enable");

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : $"namespace {symbol.ContainingNamespace.ToDisplayString()}\n{{\n";

        if (namespaceName != null)
            sb.Append(namespaceName);

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();

        sb.AppendLine($"    {accessibility} static partial class {symbol.Name}");
        sb.AppendLine("    {");
        sb.AppendLine(
            "        private static readonly Ethereal.Classes.API.DeferrableAPI API = new();"
        );

        if (!symbol.GetMembers("SetReady").OfType<IMethodSymbol>().Any())
        {
            sb.AppendLine();
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Mark the API as ready and run all deferred methods.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        internal static void SetReady() => API.SetReady();");
        }

        sb.AppendLine();
        sb.AppendLine(
            "        internal static System.Threading.Tasks.Task<bool> Task => API.TaskSource.Task;"
        );

        sb.AppendLine("    }");

        if (namespaceName != null)
            sb.AppendLine("}");

        return sb.ToString();
    }
}
