using UnityEngine;

public class CustomUnit1 : Unit
{
    public CustomUnit1(UnitManager unitManager, int teamId, int unitId, Command command) : base(unitManager, teamId, unitId, command) { }

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