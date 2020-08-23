using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiStackServiceHost.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Remove<T>(this IEnumerable<T> items, int index)
        {
            var list = new List<T>(items);
            list.Remove(list[index]);
            return list.ToArray();
        }

        public static T GetByIndex<T>(this IEnumerable<T> items, int index)
        {
            var list = new List<T>(items);
            return list[index];
        }

        public static bool IsEmpty<T>(this IEnumerable<T> items)
        {
            return !items.Any();
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> forEachAction)
        {
            foreach (var item in items)
            {
                forEachAction(item);
            }
        }
    }
}
