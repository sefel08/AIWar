using UnityEngine;
public class ParticlePoolObject
{
    public GameObject gameObject;
    public ParticleSystem particleSystem;

    public ParticlePoolObject(GameObject gameObject, ParticleSystem particleSystem)
    {
        this.gameObject = gameObject;
        this.particleSystem = particleSystem;
    }
}
