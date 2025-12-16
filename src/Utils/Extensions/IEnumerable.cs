using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ethereal.Utils.Extensions;

public static class IEnumerableExtensions
{
    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Task<TResult>> method
    ) => await Task.WhenAll(source.Select(method));

    public static async Task<IEnumerable<TResult>> SelectManyAsync<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, Task<List<TResult>>> method
    ) => (await Task.WhenAll(source.Select(method))).SelectMany(x => x);
}
