using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabSpawner : Singleton<PrefabSpawner>
{
    private readonly Dictionary<int, Stack<int>> _stackDespawnedObjectsByPrefabType = new();
    private readonly Dictionary<GameObject, CachedObjectIds> _cachedObjectsIds = new();
    
    public GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool setActive = true)
    {
        if (prefab == null)
            return null;

        // key for prefab type, to not mix prefabs
        var key = prefab.GetInstanceID();
        var stackExists = _stackDespawnedObjectsByPrefabType.TryGetValue(key, out var stack);
        
        GameObject spawnedObject = null;

        // stack exists and object can be extracted
        if (stackExists && stack?.Count > 0)
        {
            var id = stack.Pop();
            spawnedObject = _cachedObjectsIds.FirstOrDefault(x => x.Value.instanceId == id).Key;
            spawnedObject.transform.SetPositionAndRotation(position, rotation);
            spawnedObject.transform.SetParent(parent);

            // activate cached inactive object
            spawnedObject.SetActive(setActive);
            
            return spawnedObject;
        }
        
        // if stack of key doesn't exist we need to create new stack for selected key and populate new object
        if (!stackExists)
            _stackDespawnedObjectsByPrefabType[key] = new Stack<int>();

        spawnedObject = Populate(prefab, position, rotation, parent, setActive);
        _cachedObjectsIds.Add(spawnedObject, new CachedObjectIds{prefabId = key, instanceId = spawnedObject.GetInstanceID()});

        return spawnedObject;
    }
    
    private GameObject Populate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool setActive = true)
    {
        var instantiatedObject = GameObject.Instantiate(prefab, position, rotation);
        instantiatedObject.transform.SetParent(parent);
        
        instantiatedObject.SetActive(setActive);

        return instantiatedObject;
    }

    public void Despawn(GameObject go, bool destroy = false)
    {
        if (go == null)
            return;

        go.SetActive(false);
        go.transform.SetParent(null);

        if (destroy)
        {
            _cachedObjectsIds.Remove(go);
            GameObject.Destroy(go);   
            return;
        }

        //if object wasn't destroyed we need to add it to prefab type stack object ids cache
        var ids = _cachedObjectsIds[go];
        _stackDespawnedObjectsByPrefabType[ids.prefabId].Push(ids.instanceId);
    }
    
    public void DespawnAll()
    {
        foreach (var spawnedGameObject in _cachedObjectsIds)
            Despawn(spawnedGameObject.Key);
    }

    private struct CachedObjectIds
    {
        public int prefabId;
        public int instanceId;
    }
}
