using UnityEngine;

public class EnemyData
{
    public Vector2 position; // The position of the enemy in the game world
    public Quaternion rotation; // The rotation of the enemy in the game world
    public Vector2 direction; // The direction the enemy is facing in the game world

    public Vector2 bestShootingPosition; // The point on the target that, when aimed at, gives the highest chance of hitting
    public float seenConeAngle; // The angle of enemy that the unit can see. Look at the discord for more information.

    public EnemyData(UnitData enemyUnitData, Vector2 bestShootingPosition, float seenConeAngle)
    {
        this.position = enemyUnitData.Position;
        this.rotation = enemyUnitData.Rotation;
        this.direction = enemyUnitData.Direction;
        this.bestShootingPosition = bestShootingPosition;
        this.seenConeAngle = seenConeAngle;
    }
}

