using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Generators;

#nullable enable
[Generator]
public sealed class BasicAPIGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Generator.BasicAPI";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0,
                static (ctx, _) => GetClassWithAttribute(ctx)
            )
            .Where(static c => c is not null);

        context.RegisterSourceOutput(provider, static (spc, source) => Execute(spc, source!));
    }

    private static INamedTypeSymbol? GetClassWithAttribute(GeneratorSyntaxContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(declaration) is not INamedTypeSymbol symbol)
            return null;

        var serializableAttr = context.SemanticModel.Compilation.GetTypeByMetadataName(Attribute);

        if (serializableAttr == null)
            return null;

        return symbol
            .GetAttributes()
            .Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, serializableAttr))
            ? symbol
            : null;
    }

    private static void Execute(SourceProductionContext context, INamedTypeSymbol symbol)
    {
        string source = GeneratePartialClass(symbol);

        context.AddSource($"{symbol.Name}_API.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string GeneratePartialClass(INamedTypeSymbol symbol)
    {
        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : $"namespace {symbol.ContainingNamespace.ToDisplayString()}\n{{\n";

        var sb = new StringBuilder();

        if (namespaceName != null)
            sb.Append(namespaceName);

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();

        sb.AppendLine($"    {accessibility} static partial class {symbol.Name}");
        sb.AppendLine("    {");
        sb.AppendLine("        private static readonly BaseAPI API = new();");

        if (!symbol.GetMembers("SetReady").OfType<IMethodSymbol>().Any())
        {
            sb.AppendLine();
            sb.AppendLine("        /// <summary>");
            sb.AppendLine("        /// Mark the API as ready.");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine("        internal static void SetReady() => API.SetReady();");
        }

        sb.AppendLine("    }");

        if (namespaceName != null)
            sb.AppendLine("}");

        return sb.ToString();
    }
}

#nullable disable
