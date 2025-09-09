using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomCommand2 : Command<CustomUnit2>, ICommand
{
    public override void Start()
    {

    }
    public override void Update()
    {
        foreach(CustomUnit2 unit in units.Values)
        {
            unit.GetVisibleEnemies();
        }
    }
    public override void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection)
    {

    }
}
