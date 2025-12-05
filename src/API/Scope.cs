using System;
using System.Threading;

namespace Ethereal.API;

public static class Scopes
{
    private static readonly AsyncLocal<string?> _scope = new AsyncLocal<string?>();

    public static IDisposable Scope(string? scope)
    {
        var old = _scope.Value;
        _scope.Value = scope;
        return new ScopePop(old);
    }

    public static string? CurrentScope => _scope.Value;

    private class ScopePop : IDisposable
    {
        private readonly string? _old;
        public ScopePop(string? old) => _old = old;

        public void Dispose() => _scope.Value = _old;
    }
}