using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class PrivatePrimaryConstructor : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.PrivatePrimaryConstructor";

    private const string Suffix = "PrivatePrimaryCtor";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<NamedTypeMetadata?> provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is RecordDeclarationSyntax m && m.AttributeLists.Count > 0,
                static (ctx, _) => RecordHelper.GetWithAttribute(ctx, Attribute)
            )
            .Where(static c => c is not null);

        context.RegisterSourceOutput(
            provider,
            static (spc, source) =>
                GeneratorHelper.Execute<NamedTypeMetadata, INamedTypeSymbol>(
                    spc,
                    source!,
                    Suffix,
                    GeneratePartialRecord
                )
        );
    }

    private static string GeneratePartialRecord(NamedTypeMetadata symbolMetadadta)
    {
        StringBuilder sb = new();

        sb.AppendLine("#nullable enable");

        INamedTypeSymbol symbol = symbolMetadadta.Symbol;
        ImmutableArray<Member>? parameters = symbolMetadadta.Parameters;

        string namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : $"namespace {symbol.ContainingNamespace.ToDisplayString()}\n{{\n";

        if (!string.IsNullOrEmpty(namespaceName))
            sb.Append(namespaceName);

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();

        sb.AppendLine($"    {accessibility} sealed partial record {symbol.Name}");
        sb.AppendLine("    {");
        sb.AppendLine($"        private {symbol.Name}(");

        sb.AppendLine(
            $"            {string.Join(",\n            ", parameters?.Select(x => x.AsArgument()))}"
        );

        sb.AppendLine("        )");
        sb.AppendLine("        {");
        sb.AppendLine(
            $"            {string.Join(";\n            ", parameters?.Select(x => x.AsSetter()))};"
        );
        sb.AppendLine("        }");
        sb.AppendLine("    }");

        if (namespaceName != null)
            sb.AppendLine("}");

        return sb.ToString();
    }
}
