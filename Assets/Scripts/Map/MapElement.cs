using System.Collections.Generic;
using UnityEngine;

public class MapElement
{
    public readonly int ElementId;
    public List<Vector2> Points;
    public MapElementType Type;

    public MapElement(int mapElementId, List<Vector2> points, MapElementType type)
    {
        this.ElementId = mapElementId;
        Points = points;
        Type = type;
    }
}
