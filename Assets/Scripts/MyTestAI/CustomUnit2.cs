using UnityEngine;

public class CustomUnit2 : Unit<CustomUnit2>
{
    public Vector2 currentDirection;

    public override void OnUnitDeath()
    {
        command.units.Remove(UnitId);
    }
}
