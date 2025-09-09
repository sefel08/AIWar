using UnityEngine;

public class CustomUnit1 : Unit<CustomUnit1>
{
    public Vector2 targetDirection;

    public override void OnUnitDeath()
    {
        // Custom logic for when this unit dies
        Debug.Log($"Unit {UnitId} from team {TeamId} has died.");
    }

    public void TestMethod()
    {
        // Example method to demonstrate functionality
        Debug.Log($"Unit {UnitId} from team {TeamId} is testing a method.");
    }
}