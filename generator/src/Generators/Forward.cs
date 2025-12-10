using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Generators.Enums;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class ForwardGenerator : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.Forward";

    private const string Suffix = "Forward";

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

        if (IsLinqUsed(properties))
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
                Member fieldOrProperty = attributeProperties[0];
                string forwardTarget = fieldOrProperty.Name;

                if (Enum.TryParse(attributeProperties[1]?.Name, out ForwardConversion conversion))
                {
                    List<string> accessors = GenerateAccessors(forwardTarget, property, conversion);

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

    /// <summary>
    /// Check if Linq is required to import it as neccessary.
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    internal static bool IsLinqUsed(IEnumerable<PropertyMetadata> properties) =>
        properties.Any(x =>
            x.AttributeProperties?.Any(y =>
                Enum.TryParse(y.Name, out ForwardConversion conversion)
                && conversion == ForwardConversion.GameObjectList
            ) ?? false
        );

    /// <summary>
    /// Generate a forward property's getter and setter.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="property"></param>
    /// <param name="conversion"></param>
    /// <returns></returns>
    internal static List<string> GenerateAccessors(
        string target,
        IPropertySymbol property,
        ForwardConversion conversion
    ) =>
        conversion switch
        {
            ForwardConversion.None => [$"get => {target};", $"set => {target} = value;"],
            ForwardConversion.GameObject =>
            [
                $"get => {target}.GetComponent<{property.Type}>();",
                $"set => {target} = value.gameObject;",
            ],
            ForwardConversion.GameObjectList =>
            [
                $"get => [.. {target}.Select(x => x.GetComponent<{GetItemTypeFromGeneric(property.Type.ToString())}>())];",
                $"set => {target} = [.. value.Select(x => x.gameObject)];",
            ],
            _ => [""],
        };

    /// <summary>
    /// Get the type of T from Type<T>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetItemTypeFromGeneric(string type)
    {
        int start = type.LastIndexOf('<') + 1;
        int length = type.IndexOf('>') - start;
        return type.Substring(start, length);
    }
}
