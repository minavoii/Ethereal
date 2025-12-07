using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class ForwardToGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.ForwardTo";

    private const string Suffix = "ForwardTo";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<PropertyMetadata> provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                    node is PropertyDeclarationSyntax m && m.AttributeLists.Count > 0,
                static (ctx, _) => ArgumentHelper.GetWithAttribute(ctx, Attribute)!
            )
            .Where(static m => m is not null);

        IncrementalValueProvider<(
            Compilation Left,
            ImmutableArray<PropertyMetadata> Right
        )> compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(
            compilation,
            static (spc, source) =>
                GeneratorHelper.ExecuteGroup<PropertyMetadata, IPropertySymbol>(
                    spc,
                    source.Right!,
                    Suffix,
                    GeneratePartialClass
                )
        );
    }

    private static string GeneratePartialClass(
        ISymbol symbol,
        IEnumerable<PropertyMetadata> properties
    )
    {
        StringBuilder sb = new();

        sb.AppendLine("#nullable enable");

        if (ForwardGenerator.IsLinqUsed(properties))
            sb.AppendLine("using System.Linq;");

        string @namespace = symbol.ContainingNamespace.ToDisplayString();

        if (!string.IsNullOrEmpty(@namespace))
        {
            sb.AppendLine($"namespace {@namespace}");
            sb.AppendLine("{");
        }

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();
        sb.AppendLine($"    {accessibility} sealed partial class {symbol.Name}");
        sb.AppendLine("    {");

        foreach (PropertyMetadata data in properties)
        {
            IPropertySymbol property = data.Symbol;

            if (
                data.AttributeProperties.HasValue
                && data.AttributeProperties.Value is ImmutableArray<Member> attributeProperties
            )
            {
                Member classInstance = attributeProperties[0];
                string forwardTarget = $"{classInstance.Name}.{property.Name}";

                if (Enum.TryParse(attributeProperties[1]?.Name, out ForwardConversion conversion))
                {
                    List<string> accessors = ForwardGenerator.GenerateAccessors(
                        forwardTarget,
                        property,
                        conversion
                    );

                    sb.AppendLine($"        public partial {property.Type} {property.Name}");
                    sb.AppendLine("        {");

                    foreach (string line in accessors)
                        sb.AppendLine($"            {line}");

                    sb.AppendLine("        }");
                }
            }
        }

        // Class ends
        sb.AppendLine("    }");

        if (!string.IsNullOrEmpty(@namespace))
            sb.AppendLine("}");

        return sb.ToString();
    }
}
