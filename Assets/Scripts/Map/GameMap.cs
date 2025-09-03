using System.Collections.Generic;
using UnityEngine;

public class GameMap
{
    public Dictionary<int, MapElement> Elements;

    public GameMap()
    {
        Elements = new Dictionary<int, MapElement>();
    }
    //deep copy constructor
    public GameMap(GameMap original)
    {
        Elements = new Dictionary<int, MapElement>();
        foreach (var elementWithId in original.Elements)
        {
            MapElement element = elementWithId.Value;
            List<Vector2> pointsCopy = new List<Vector2>(element.Points);
            MapElementType typeCopy = element.Type;
            MapElement newElement = new MapElement(element.ElementId, pointsCopy, typeCopy);
            Elements.Add(elementWithId.Key, newElement);
        }
    }
}
