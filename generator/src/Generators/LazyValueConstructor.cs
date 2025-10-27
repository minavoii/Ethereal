using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generators.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Generators;

[Generator]
public sealed class LazyValueConstructor : IIncrementalGenerator
{
    private const string Attribute = "Ethereal.Attributes.LazyValueConstructor";

    private const string Property = "NamedTypes";

    private const string Suffix = "LazyValueCtor";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ConstructorDeclarationSyntax,
                static (ctx, _) =>
                    ConstructorHelper.GetWithAttributeAndProperty(ctx, Attribute, Property)
            )
            .Where(static m => m is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());

        context.RegisterSourceOutput(
            compilation,
            (spc, source) =>
                GeneratorHelper.ExecuteGroup<ConstructorMetadata, IMethodSymbol>(
                    spc,
                    source.Right!,
                    Suffix,
                    GeneratePartialClass
                )
        );
    }

    private string GeneratePartialClass(ISymbol symbol, IEnumerable<ConstructorMetadata> methods)
    {
        var sb = new StringBuilder();

        sb.AppendLine("#nullable enable");

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : $"namespace {symbol.ContainingNamespace.ToDisplayString()}\n{{\n";

        if (namespaceName != null)
            sb.Append(namespaceName);

        string accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();

        sb.AppendLine($"    {accessibility} sealed partial record {symbol.Name}");
        sb.AppendLine("    {");

        foreach (ConstructorMetadata method in methods)
        {
            if (method.AttributeProperties == null)
                continue;

            foreach (var property in method.AttributeProperties)
                sb.Append(GenerateConstructor(symbol, method, property));
        }

        sb.AppendLine("    }");

        if (namespaceName != null)
            sb.AppendLine("}");

        return sb.ToString();
    }

    private string GenerateConstructor(ISymbol symbol, ConstructorMetadata method, Member property)
    {
        if (method.Arguments?.FirstOrDefault() is not string lazyArg)
            return "";

        var sb = new StringBuilder();

        var lazyType = lazyArg.StartsWith("new")
            ? lazyArg.Substring(0, lazyArg.IndexOf('('))
            : "new " + lazyArg;

        IEnumerable<string> lazyParameter = [$"{lazyType}({property.Name})"];
        IEnumerable<Member> properties = [property, .. method.Parameters?.Skip(1) ?? []];
        IEnumerable<string> parameterNames = lazyParameter.Concat(method.Arguments?.Skip(1));

        sb.AppendLine($"        public {symbol.Name}(");
        sb.AppendLine(
            $"            {string.Join(",\n            ", properties.Select(x => x.AsArgument()))}"
        );
        sb.AppendLine($"        ): this({string.Join(", ", parameterNames)}) {{}}");

        return sb.ToString();
    }
}
