using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Generators;

internal sealed record MethodMetadata(
    IMethodSymbol Symbol,
    ImmutableArray<Member>? Parameters = null,
    List<string>? Comments = null,
    ImmutableArray<Member>? AttributeProperties = null
) : SymbolMetadata<IMethodSymbol>(Symbol, AttributeProperties);
