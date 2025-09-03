using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<TComponent>
    where TComponent : Component
{
    public List<PoolObject<TComponent>> pool;
    
    private Transform parent;
    private GameObject prefab;
    private bool getComponent;

    public GameObjectPool(Transform parent, string containerName, GameObject prefab, bool getComponent)
    {
        pool = new List<PoolObject<TComponent>>();
        GameObject parentGameObject = new GameObject(containerName);
        parentGameObject.transform.parent = parent;
        this.parent = parentGameObject.transform;
        this.prefab = prefab;
        this.getComponent = getComponent;
    }

    public PoolObject<TComponent> GetObjectFromPool()
    {
        PoolObject<TComponent> poolObject;

        if (pool.Count > 0)
        {
            poolObject = pool[0];
            poolObject.gameObject.SetActive(true);
            pool.RemoveAt(0);
            return poolObject;
        }

        GameObject newGameObject = GameObject.Instantiate(prefab, parent);
        TComponent script = (getComponent) ? newGameObject.GetComponent<TComponent>() : newGameObject.AddComponent<TComponent>();
        poolObject = new PoolObject<TComponent>(newGameObject, script, this);
        return poolObject;
    }
    public IPoolObject GetInterfaceFromPool()
    {
        return GetObjectFromPool();
    }
}