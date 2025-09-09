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

    public bool isAlive;

    public UnitInfo(UnitData unitData, float zoneDistance)
    {
        this.position = unitData.Position;
        this.rotation = unitData.Rotation;
        this.direction = unitData.Direction;
        this.health = unitData.health;
        this.hasMoved = unitData.hasMoved;
        this.hasRotated = unitData.hasRotated;
        this.zoneDistance = zoneDistance;
        this.isAlive = unitData.isAlive;
    }

    public UnitInfo(bool isAlive)
    {
        this.isAlive = isAlive;
    }
}

