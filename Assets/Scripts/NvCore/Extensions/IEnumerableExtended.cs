using UnityEngine;
using System.Linq;

namespace System.Collections.Generic
{
    public static class IEnumerableExt
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || items.Count() == 0;
        }

        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        public static int FindLastIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retval = -1;
            int index = 0;
            foreach (var item in items)
            {
                if (predicate(item)) retval = index;
                index++;
            }
            return retval;
        }

        public static T FindLast<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            T retval = default(T);
            foreach (var item in items)
            {
                if (predicate(item)) retval = item;
            }
            return retval;
        }

        /// <summary>
        /// Borrowed from http://stackoverflow.com/questions/1955766/iterate-two-lists-or-arrays-with-one-foreach-statment-in-c-sharp
        /// </summary>
        public static IEnumerable<KeyValuePair<T, U>> Zip<T, U>(IEnumerable<T> first, IEnumerable<U> second)
        {
            IEnumerator<T> firstEnumerator = first.GetEnumerator();
            IEnumerator<U> secondEnumerator = second.GetEnumerator();

            while (firstEnumerator.MoveNext())
            {
                if (secondEnumerator.MoveNext())
                {
                    yield return new KeyValuePair<T, U>(firstEnumerator.Current, secondEnumerator.Current);
                }
                else
                {
                    yield return new KeyValuePair<T, U>(firstEnumerator.Current, default(U));
                }
            }
            while (secondEnumerator.MoveNext())
            {
                yield return new KeyValuePair<T, U>(default(T), secondEnumerator.Current);
            }
        }

        /// <summary>
        /// Compares the values in two IEnumerables.
        /// </summary>
        public static bool CompareValues<T>(this IEnumerable<T> self, IEnumerable<T> other)// where T : IComparable
        {
            if (self == null) throw new ArgumentNullException("self");
            if (other == null) throw new ArgumentNullException("other");

            IEnumerator<T> firstEnumerator = self.GetEnumerator();
            IEnumerator<T> secondEnumerator = other.GetEnumerator();

            while (firstEnumerator.MoveNext())
            {
                if (!secondEnumerator.MoveNext())
                {
                    return false;
                }
                else if (!firstEnumerator.Current.Equals(secondEnumerator.Current))
                {
                    return false;
                }
            }

            return !secondEnumerator.MoveNext();
        }
    }
}
