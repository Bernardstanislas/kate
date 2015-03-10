using System;
using System.Collections.Generic;

namespace Kate.Utils
{
    public static class Extensions
    {
        public static Func<A, R> Memoize<A, R>(this Func<A, R> f)
        {
            var map = new Dictionary<A, R>();
            return a =>
            {
                R value;
                if (map.TryGetValue(a, out value))
                    return value;
                value = f(a);
                map.Add(a, value);
                return value;
            };
        }

        public static Func<A, B, R> Memoize<A, B, R>(this Func<A, B, R> f)
        {
            var map = new Dictionary<Tuple<A, B>, R>();
            return (a, b) =>
            {
                R value;
                if (map.TryGetValue(Tuple.Create(a, b), out value))
                    return value;
                value = f(a, b);
                map.Add(Tuple.Create(a, b), value);
                return value;
            };
        }
    }
}
