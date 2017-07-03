using UnityEngine;
using System.Collections.Generic;

public static class EnumExt
{
    /// <summary>
    /// Returns iterator over all the values in the Enum.
    /// </summary>
    public static IEnumerable<T> GetValues<T>()
    {
        return (T[])System.Enum.GetValues(typeof(T));
    }

    /// <summary>
    /// Determines whether this instance has flag the specified val flags.
    /// WARNING:
    /// The ToInt64() will likely box and unbox values, this will not be the most performant
    /// </summary>
    public static bool HasFlag<T>(T val, T flags) where T : struct, System.IConvertible
    {
        System.Int64 flags64 = flags.ToInt64(null);
        return (val.ToInt64(null) & flags64) == flags64;
    }

    public static T Parse<T>(string value, bool ignoreCase = true)
    {
        return (T)System.Enum.Parse(typeof(T), value, ignoreCase);
    }
}