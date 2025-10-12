using System;
using Microsoft.CodeAnalysis;

namespace Generators;

#nullable enable
internal sealed class SymbolWithComments()
{
    public string Comments { get; init; } = String.Empty;

    public IMethodSymbol? Symbol { get; init; }
}
#nullable disable
