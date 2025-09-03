using UnityEngine;

public class MapElementData
{
    public MapElement mapElement;
    public GameObject gameObject;
    public Vector2 position => gameObject.transform.position;
    public float radius;

    public MapElementData(MapElement mapElement, GameObject mapElementGameObject, float radius)
    {
        this.mapElement = mapElement;
        this.gameObject = mapElementGameObject;
        this.radius = radius;
    }
}
