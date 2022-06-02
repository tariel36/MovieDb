using System.Collections;
using System.Text.RegularExpressions;

namespace MovieDbApi.Common.Domain.Utility
{
    public static class Extensions
    {
        public static void SafeDispose(IDisposable disposable)
        {
            try
            {
                disposable?.Dispose();
            }
            catch
            {
                // Ignored
            }
        }

        public static string GetNonEmpty(params object[] objs)
        {
            List<string> items = new List<string>();

            foreach (object obj in objs)
            {
                if (obj is IEnumerable enumerable && obj is not string)
                {
                    items.AddRange(enumerable.Cast<object>().Select(x => x.ToString()).ToList());
                }
                else if (obj != null)
                {
                    items.Add(obj.ToString());
                }
            }

            return items.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? "";
        }

        public static HashSet<TValue> ToHashSet<TItem, TValue>(this IEnumerable<TItem> enumerable, Func<TItem, TValue> selector, bool throwOnDuplicate = false)
        {
            HashSet<TValue> result = new HashSet<TValue>();

            foreach (TItem item in enumerable)
            {
                TValue key = selector(item);

                if (!result.Add(key) && throwOnDuplicate)
                {
                    throw new InvalidOperationException($"Key {key} already exists.");
                }
            }

            return result;
        }

        public static string Join(this IEnumerable<string> items, string separator)
        {
            return string.Join(separator, items);
        }

        public static string Join(this IEnumerable<char> items, string separator)
        {
            return string.Join(separator, items);
        }

        public static void ForEach<TValue>(this IEnumerable<TValue> enumerable, Action<TValue> action)
        {
            if (enumerable == null)
            {
                return;
            }

            action ??= (x) => {};

            foreach (TValue item in enumerable)
            {
                action(item);
            }
        }

        public static string Replace(this string str, Regex regex, string replace)
        {
            return regex.Replace(str, replace);
        }
    }
}
