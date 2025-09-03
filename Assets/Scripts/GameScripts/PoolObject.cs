using UnityEngine;

public class PoolObject<TComponent> : IPoolObject
    where TComponent : Component
{
    public GameObject gameObject { get; private set; }
    public TComponent component { get; private set; }

    GameObjectPool<TComponent> gameObjectPool;

    public PoolObject(GameObject gameObject, TComponent script, GameObjectPool<TComponent> gameObjectPool)
    {
        this.gameObject = gameObject;
        this.component = script;
        this.gameObjectPool = gameObjectPool;
    }

    public void ReturnObject()
    {
        gameObject.SetActive(false);
        gameObjectPool.pool.Add(this);
    }
}