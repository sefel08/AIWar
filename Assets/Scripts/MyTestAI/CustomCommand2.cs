using UnityEngine;
using System.Collections.Generic;

public class CustomCommand2 : Command<CustomUnit2>, ICommand
{
    public override void Start()
    {
        foreach (var unit in units.Values)
        {
            unit.currentDirection = GetRandomDirection();
        }
    }

    public override void Update()
    {
        foreach (var unit in units.Values)
        {
            if (unit.ZoneDistance < 1f)
            {
                unit.currentDirection = (Vector2.zero - unit.Position).normalized;
                unit.Move(unit.currentDirection);
                continue;
            }

            unit.RotateTowards(unit.currentDirection);

            List<EnemyData> visibleEnemies = unit.GetVisibleEnemies();
            if (visibleEnemies.Count > 0)
            {
                unit.currentDirection = (visibleEnemies[0].bestShootingPosition - unit.Position).normalized;
                unit.Shoot();
                continue;
            }

            HitType hitType = unit.CastARay(unit.currentDirection, 2f, out HitData hitData);
            if(hitType == HitType.Map || hitType == HitType.Friendly)
            {
                unit.currentDirection = GetRandomDirection();
            }

            unit.Move(unit.currentDirection);
        }
    }

    private Vector2 GetRandomDirection()
    {
        return Random.insideUnitCircle.normalized;
    }
}
