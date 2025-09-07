using UnityEngine;

public interface IPoolObject
{
    public GameObject gameObject { get; }
    public void ReturnObject();
}

