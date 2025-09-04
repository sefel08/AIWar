using UnityEngine;

public class HitData
{
    public readonly Vector2 hitPoint;
    public readonly float hitDistance;
    public readonly Vector2 hitNormal;
    public readonly int elementId;

    public HitData(Vector2 hitPoint, float hitDistance, Vector2 hitNormal, int elementId)
    {
        this.hitPoint = hitPoint;
        this.hitDistance = hitDistance;
        this.hitNormal = hitNormal;
        this.elementId = elementId;
    }
    public override string ToString()
    {
        return $"HitPoint: {hitPoint}, HitDistance: {hitDistance}, HitNormal: {hitNormal}, ElementId: {elementId}";
    }
}
