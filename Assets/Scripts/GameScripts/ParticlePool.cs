using System.Collections.Generic;
using UnityEngine;

public class ParticlePool
{
    private GameObject particlePrefab;
    private Transform parent;

    private List<ParticlePoolObject> pool;

    public ParticlePool(Transform parent, string containerName, GameObject particlePrefab)
    {
        this.particlePrefab = particlePrefab;
        GameObject parentGameObject = new GameObject(containerName);
        parentGameObject.transform.parent = parent;
        this.parent = parentGameObject.transform;
        pool = new List<ParticlePoolObject>();
    }

    public void SpawnParticle(Vector2 postion, Quaternion rotation)
    {
        if (pool.Count > 0)
        {
            foreach (var obj in pool)
            {
                if (obj.gameObject.activeSelf) continue;
                
                obj.gameObject.SetActive(true);
                obj.gameObject.transform.position = postion;
                obj.gameObject.transform.rotation = rotation;
                return;
            }
        }

        GameObject particleGameObject = GameObject.Instantiate(particlePrefab, postion, rotation, parent);
        ParticleSystem particleSystem = parent.GetComponent<ParticleSystem>();
        ParticlePoolObject particlePoolObject = new ParticlePoolObject(particleGameObject, particleSystem);
        pool.Add(particlePoolObject);
    }
}