using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Generators;

internal sealed record ConstructorMetadata(
    IMethodSymbol Symbol,
    ImmutableArray<Member>? Parameters = null,
    ImmutableArray<string>? Arguments = null,
    string? Comments = null,
    ImmutableArray<Member>? AttributeProperties = null
) : SymbolMetadata<IMethodSymbol>(Symbol, AttributeProperties);
