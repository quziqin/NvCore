using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    private Dictionary<string, GameObject> _cachedPrefabs = new Dictionary<string, GameObject>();
    
    private Dictionary<string, List<GameObject>> _cachedGameObjects = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, int> _cachedGameObjectsAvailableCount = new Dictionary<string, int>();
    private List<string> _prefabWaitingForCache = new List<string>();

    private Transform _objectPoolRoot = null;
    private Transform ObjectPoolRoot
    {
        get
        {
            if (_objectPoolRoot != null) return _objectPoolRoot;

            GameObject go = new GameObject("ObjectPool");
            _objectPoolRoot = go.GetComponent<Transform>() ?? go.AddComponent<Transform>();
            _objectPoolRoot.position = Vector3.zero;
            _objectPoolRoot.localScale = Vector3.one;

            return _objectPoolRoot;
        }
    }

    private Dictionary<string, Transform> _cachedPrefabRoot = new Dictionary<string, Transform>();

    public void Awake()
    {
        
    }

    public void OnDestroy()
    {
        
    }

    public void Clear()
    {
        _cachedGameObjects.Clear();
        _cachedGameObjectsAvailableCount.Clear();
        _prefabWaitingForCache.Clear();
    }

    public GameObject GetCachedPrefab(string abid, bool ignoreCase = false)
    {
        if (_cachedPrefabs.ContainsKey(abid))
        {
            return _cachedPrefabs[abid];
        }
        else if (ignoreCase)
        {
            foreach (var kv in _cachedPrefabs)
            {
                if (string.Equals(kv.Key, abid, StringComparison.CurrentCultureIgnoreCase))
                {
                    return kv.Value;
                }
            }
        }

        return null;
    }
}
