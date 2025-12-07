using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Generators.Helpers;

internal static class GeneratorHelper
{
    /// <summary>
    /// Add sources for all matching symbols and generate a different output for each.
    /// </summary>
    /// <typeparam name="TSymbol"></typeparam>
    /// <param name="context"></param>
    /// <param name="symbol"></param>
    /// <param name="suffix"></param>
    /// <param name="method"></param>
    internal static void Execute<TSymbol>(
        SourceProductionContext context,
        TSymbol symbol,
        string suffix,
        Func<TSymbol, string> method
    )
        where TSymbol : ISymbol
    {
        string source = method(symbol);

        context.AddSource($"{symbol.Name}_{suffix}.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    /// <summary>
    /// Add sources for all matching symbols and generate a different output for each.
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <typeparam name="TSymbol"></typeparam>
    /// <param name="context"></param>
    /// <param name="metadata"></param>
    /// <param name="suffix"></param>
    /// <param name="method"></param>
    internal static void Execute<TMetadata, TSymbol>(
        SourceProductionContext context,
        TMetadata metadata,
        string suffix,
        Func<TMetadata, string> method
    )
        where TMetadata : SymbolMetadata<TSymbol>
        where TSymbol : ISymbol
    {
        string source = method(metadata);

        context.AddSource(
            $"{metadata.Symbol.Name}_{suffix}.g.cs",
            SourceText.From(source, Encoding.UTF8)
        );
    }

    /// <summary>
    /// Add a single source for all matching symbols of the same type.
    /// </summary>
    /// <typeparam name="TSymbol"></typeparam>
    /// <param name="spc"></param>
    /// <param name="symbols"></param>
    /// <param name="suffix"></param>
    /// <param name="method"></param>
    internal static void ExecuteGroup<TSymbol>(
        SourceProductionContext spc,
        ImmutableArray<TSymbol> symbols,
        string suffix,
        Func<ISymbol, IEnumerable<TSymbol>, string> method
    )
        where TSymbol : ISymbol
    {
        if (symbols.IsDefaultOrEmpty)
            return;

        // Emit a single partial class
        IEnumerable<IGrouping<ISymbol?, TSymbol>> classes = symbols.GroupBy(
            m => m.ContainingType,
            SymbolEqualityComparer.Default
        );

        foreach (IGrouping<ISymbol?, TSymbol> partialClass in classes)
        {
            ISymbol type = partialClass.Key!;

            spc.AddSource(
                $"{type.Name}_{suffix}.g.cs",
                SourceText.From(method(type, partialClass), Encoding.UTF8)
            );
        }
    }

    /// <summary>
    /// Add a single source for all matching symbols of the same type.
    /// </summary>
    /// <typeparam name="TMetadata"></typeparam>
    /// <typeparam name="TSymbol"></typeparam>
    /// <param name="spc"></param>
    /// <param name="metadataList"></param>
    /// <param name="suffix"></param>
    /// <param name="method"></param>
    internal static void ExecuteGroup<TMetadata, TSymbol>(
        SourceProductionContext spc,
        ImmutableArray<TMetadata> metadataList,
        string suffix,
        Func<ISymbol, IEnumerable<TMetadata>, string> method
    )
        where TMetadata : SymbolMetadata<TSymbol>
        where TSymbol : ISymbol
    {
        if (metadataList.IsDefaultOrEmpty)
            return;

        // Emit a single partial class
        IEnumerable<IGrouping<ISymbol?, TMetadata>> classes = metadataList.GroupBy(
            m => m.Symbol.ContainingType,
            SymbolEqualityComparer.Default
        );

        foreach (IGrouping<ISymbol?, TMetadata> partialClass in classes)
        {
            ISymbol type = partialClass.Key!;

            spc.AddSource(
                $"{type.Name}_{suffix}.g.cs",
                SourceText.From(method(type, partialClass), Encoding.UTF8)
            );
        }
    }
}
