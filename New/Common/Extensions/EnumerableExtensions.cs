using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> f)
        {
            return source?.SelectMany(c => f(c).Flatten(f)).Concat(source);
        }
    }
}
