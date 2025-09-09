using UnityEngine;

public class CustomUnit2 : Unit<CustomUnit2>
{
    public override void OnUnitDeath()
    {
        command.units.Remove(UnitId);
    }
}
