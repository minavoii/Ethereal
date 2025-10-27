using System.Collections.Immutable;

namespace Generators;

internal abstract record SymbolMetadata<T>(
    T Symbol,
    ImmutableArray<Member>? AttributeProperties = null
);
