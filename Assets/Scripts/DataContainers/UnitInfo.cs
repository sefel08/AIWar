using UnityEngine;

public class UnitInfo
{
    public readonly Vector2 position;
    public readonly Quaternion rotation;
    public readonly Vector2 direction;
    public readonly float health;
    public readonly bool hasMoved;
    public readonly bool hasRotated;
    public readonly float zoneDistance;
    public readonly bool canShoot;
    public readonly float shootCooldown;

    public readonly bool isAlive;

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
        this.canShoot = unitData.canShoot;
        this.shootCooldown = unitData.shootCooldown;
    }

    public UnitInfo(bool isAlive)
    {
        this.isAlive = isAlive;
    }
}

