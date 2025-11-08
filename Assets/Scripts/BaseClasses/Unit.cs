using System.Collections.Generic;
using UnityEngine;

public class Unit<TUnit> : IUnit
    where TUnit : Unit<TUnit>
{
    private UnitManager unitManager;

    /// <summary>
    /// The unique identifier for this unit.
    /// </summary>
    public int UnitId { get; private set; }
    /// <summary>
    /// The team identifier to which this unit belongs.
    /// </summary>
    public int TeamId { get; private set; }
    /// <summary>
    /// The command in which this unit is.
    /// </summary>
    public Command<TUnit> command;
    /// <summary>
    /// All information that is below that function (Position, Rotation, Direction, ZoneDistance...).
    /// </summary>
    public UnitInfo Info { get { return unitManager.GetUnitInfo(this); } }
    /// <summary>
    /// The current position of the unit.
    /// </summary>
    /// <remarks>
    /// The position is only a representation of the unit's actual location in the game world. Changing it won't have any impact on unit's position.
    /// </remarks>
    public Vector2 Position { get { return unitManager.GetUnitPosition(this); } }
    /// <summary>
    /// The current rotation of the unit.
    /// </summary>
    /// <remarks>
    /// The rotation is only a representation of the unit's actual rotation in the game world. Changing it won't have any impact on unit's rotation.
    /// </remarks>
    public Quaternion Rotation { get { return unitManager.GetUnitRotation(this); } }
    /// <summary>
    /// The current direction of the unit.
    /// </summary>
    /// <remarks>
    /// The direction is only a representation of the unit's actual direction in the game world. Changing it won't have any impact on unit's direction.
    /// </remarks>
    public Vector2 Direction { get { return unitManager.GetUnitRotation(this) * Vector2.right; } }
    /// <summary>
    /// Distance from the unit to the zone
    /// </summary>
    /// <remarks>
    /// The distance is only a representation of the unit's actual distance in the game world. Changing it won't have any impact on unit's distace to zone.
    /// </remarks>
    public float ZoneDistance { get { return unitManager.GetUnitZoneDistance(this); } }
    /// <summary>
    /// Returns true if the unit has moved this turn.
    /// </summary>
    public bool HasMoved { get { return unitManager.GetUnitHasMoved(this); } }
    /// <summary>
    /// Returns true if the unit has rotated this turn.
    /// </summary>
    public bool HasRotated { get { return unitManager.GetUnitHasRotated(this); } }
    /// <summary>
    /// Returns the current health of the unit.
    /// </summary>
    public float Health { get { return unitManager.GetUnitHealth(this); } }
    /// <summary>
    /// Returns true if the unit is alive, false otherwise.
    /// </summary>
    public bool IsAlive { get { return unitManager.IsUnitAlive(this); } }
    /// <summary>
    /// Returns true if the unit can shoot this turn.
    /// </summary>
    public bool CanShoot { get { return unitManager.CanUnitShoot(this); } }
    /// <summary>
    /// Returns the shoot cooldown of the unit in seconds.
    /// </summary>
    public float ShootCooldown { get { return unitManager.GetUnitShootCooldown(this); } }

    /// <summary>
    /// Function that is called when unit dies.
    /// </summary>
    public virtual void OnUnitDeath() { }

    /// <summary>
    /// Moves the unit towards the specified target position.
    /// </summary>
    /// <param name="targetPosition">The target position.</param>
    /// <returns>Returns true if the unit has moved.</returns>
    public bool MoveTowards(Vector2 targetPosition)
    {
        return unitManager.MoveUnitTowards(this, targetPosition);
    }
    /// <summary>
    /// Moves the unit in the given direction.
    /// </summary>
    /// <param name="direction">The direction to move.</param>
    /// <returns>Returns true if the unit has moved.</returns>
    public bool Move(Vector2 direction)
    {
        return unitManager.MoveUnitInDirection(this, direction);
    }
    /// <summary>
    /// Rotates the unit towards the specified rotation.
    /// </summary>
    /// <param name="targetRotation">The target rotation.</param>
    /// <returns>Returns true if the unit has rotated.</returns>
    public bool RotateTowards(Quaternion targetRotation)
    {
        return unitManager.RotateUnitTowards(this, targetRotation);
    }
    /// <summary>
    /// Rotates the unit towards the given direction.
    /// </summary>
    /// <param name="direction">The direction to rotate towards.</param>
    /// <returns>Returns true if the unit has rotated.</returns>
    public bool RotateTowards(Vector2 direction)
    {
        return unitManager.RotateUnitTowards(this, direction);
    }
    /// <summary>
    /// Rotates the unit towards the specified point.
    /// </summary>
    /// <param name="point">The point to rotate towards.</param>
    /// <returns>Returns true if the unit has rotated.</returns>
    public bool RotateTowardsPoint(Vector2 point)
    {
        return unitManager.RotateUnitTowardsPoint(this, point);
    }
    /// <summary>
    /// Rotates the unit in the specified direction (clockwise or counterclockwise).
    /// </summary>
    /// <param name="clockwise">If true, rotates the unit clockwise; otherwise, rotates counterclockwise.</param>
    /// <returns>Returns true if the unit has rotated.</returns>
    public bool Rotate(bool clockwise)
    {
        return unitManager.RotateUnit(this, clockwise);
    }
    /// <summary>
    /// Returns a list of points where enemies are visible within the unit's field of view.
    /// </summary>
    /// <returns>List of positions of enemies visible to this unit.</returns>
    public List<EnemyData> GetVisibleEnemies()
    {
        return unitManager.GetEnemiesInSight(this);
    }
    /// <summary>
    /// Shoots in front of the unit.
    /// </summary>
    public void Shoot()
    {
        unitManager.Shoot(this);
    }
    /// <summary>
    /// Casts a ray from the unit in the specified direction, and returns what was hit.
    /// </summary>
    /// <param name="direction">The direction in which to cast the ray.</param>
    /// <param name="hitData">Output parameter that will contain all info about the hit, or null.</param>
    /// <returns>Object that represents what was hit</returns>    
    public HitType CastARay(Vector2 direction, out HitData hitData)
    {
        var hit = unitManager.CastARay(this, direction);
        hitData = hit.Item2;
        return hit.Item1;
    }
    /// <summary>
    /// Casts a ray from the unit in the specified direction and distance, and returns what was hit.
    /// </summary>
    /// <param name="direction">The direction in which to cast the ray.</param>
    /// <param name="distance">The distance to cast the ray.</param>
    /// <param name="hitData">Output parameter that will contain all info about the hit, or null.</param>
    /// <returns>Object that represents what was hit</returns>
    public HitType CastARay(Vector2 direction, float distance, out HitData hitData)
    {
        var hit = unitManager.CastARay(this, direction, distance);
        hitData = hit.Item2;
        return hit.Item1;
    }
    /// <summary>
    /// Casts a ray from the unit towards a specific point and returns what was hit.
    /// </summary>
    /// <param name="point">The point towards which to cast the ray.</param>
    /// <param name="useDistanceToPoint">If true, the ray will be cast only to the point, otherwise it will fly all the way to the end of the map</param>
    /// <param name="hitData">Output parameter that will contain all info about the hit, or null.</param>
    /// <returns>Object that represents what was hit</returns>    
    public HitType CastARayTowardsAPoint(Vector2 point, bool useDistanceToPoint, out HitData hitData)
    {
        (HitType, HitData) hit = unitManager.CastARayTowardsPoint(this, point, useDistanceToPoint);
        hitData = hit.Item2;
        return hit.Item1;
    }

    /// <summary>
    /// Determines whether the specified direction is within the unit's field of view.
    /// </summary>
    /// <param name="direction">The direction to evaluate, represented as a 2D vector.</param>
    /// <returns><see langword="true"/> if the specified direction is within the unit's field of view;  otherwise, <see
    /// langword="false"/>.</returns>
    public bool IsDirectionInFieldOfView(Vector2 direction)
    {
        return unitManager.IsDirectionInUnitFieldOfView(this, direction);
    }
    /// <summary>
    /// Determines whether the specified point is within the field of view of the current unit.
    /// </summary>
    /// <param name="point">The point to check, represented as a 2D vector.</param>
    /// <returns><see langword="true"/> if the specified point is within the field of view of the current unit; otherwise, <see
    /// langword="false"/>.</returns>
    public bool IsPointInFieldOfView(Vector2 point)
    {
        return unitManager.IsPointInUnitFieldOfView(this, point);
    }
}
