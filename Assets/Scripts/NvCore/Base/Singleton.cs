using UnityEngine;
using System.Collections;

/// <summary>
/// a singleton class for non-monobehaviour object, don't use it unless you're really necessary.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : class, new()
{
    private static T _instance = null;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                _instance = _instance ?? new T();
            }

            return _instance;
        }
    }
}