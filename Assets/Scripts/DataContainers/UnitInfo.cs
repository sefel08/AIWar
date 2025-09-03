using UnityEngine;

public class UnitInfo
{
    public Vector2 position;
    public Quaternion rotation;
    public Vector2 direction;
    public float health;
    public bool hasMoved;
    public bool hasRotated;
    public float zoneDistance;

    public UnitInfo(UnitData unitData)
    {
        this.position = unitData.Position;
        this.rotation = unitData.Rotation;
        this.direction = unitData.Direction;
        this.health = unitData.health;
        this.hasMoved = unitData.hasMoved;
        this.hasRotated = unitData.hasRotated;
        this.zoneDistance = unitData.unit.ZoneDistance;
    }
}

