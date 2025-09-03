using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameMapData
{
    public GameObject mapContainer;
    public Dictionary<MapElement, MapElementData> elementsWithElementData;
    public Dictionary<GameObject, MapElementData> gameObjectWithElementDatas;

    public GameMapData(GameObject mapContainer)
    {
        this.mapContainer = mapContainer;
        elementsWithElementData = new Dictionary<MapElement, MapElementData>();
        gameObjectWithElementDatas = new Dictionary<GameObject, MapElementData>();
    }
}
