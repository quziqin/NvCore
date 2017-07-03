using UnityEngine;
using System.Collections;

namespace System.Collections.Generic
{
    public static class ListExt
    {
        public static T Last<T>(this List<T> self)
        {
            return self[self.Count - 1];
        }

        public static T PopFront<T>(this List<T> array)
        {
            T retval = array[0];
            array.RemoveAt(0);
            return retval;
        }

        public static T PopBack<T>(this List<T> array)
        {
            int index = array.Count - 1;
            T retval = array[index];
            array.RemoveAt(index);
            return retval;
        }
        public static T Pop<T>(this List<T> array)
        {
            return PopBack(array);
        }

        /// <summary>
        /// Adds an item to the list if it doesn't already exist.
        /// </summary>
        /// <returns>
        /// True if the item was added, otherwise false.
        /// </returns>
        public static bool AddUnique<T>(this List<T> self, T val)
        {
            bool contains = self.Contains(val);
            if (!contains)
            {
                self.Add(val);
            }
            return !contains;
        }

        public static bool AddUnique<T>(this List<T> self, List<T> vals)
        {
            if(vals == null)
                return false;

            for (int i = 0; i < vals.Count; i++)
                self.AddUnique<T>(vals[i]);

            return true;
        }
    }
}