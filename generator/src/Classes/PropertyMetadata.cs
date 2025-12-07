using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Generators;

internal sealed record PropertyMetadata(
    IPropertySymbol Symbol,
    ImmutableArray<Member>? AttributeProperties = null
) : SymbolMetadata<IPropertySymbol>(Symbol, AttributeProperties);
