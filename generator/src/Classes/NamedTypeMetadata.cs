using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Generators;

internal sealed record NamedTypeMetadata(
    INamedTypeSymbol Symbol,
    ImmutableArray<Member>? Parameters = null,
    string? Comments = null,
    ImmutableArray<Member>? AttributeProperties = null
) : SymbolMetadata<INamedTypeSymbol>(Symbol, AttributeProperties);
