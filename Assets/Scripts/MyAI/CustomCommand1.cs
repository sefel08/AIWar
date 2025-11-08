using System.Collections.Generic;
using UnityEngine;

public class CustomCommand1 : Command<CustomUnit1>, ICommand
{
    public override void Start()
    {
        //event called when the command starts
    }
    public override void Update()
    {
        foreach (var unit in units.Values)
        {
            UnitInfo info = unit.Info;
            if (info.isAlive == false) continue;

            //example of looping through all units
        }
    }
    public override void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection)
    {
        //event called when one of your units hears a shot
    }
    public override void OnRedCard()
    {
        //event called when command gets a red card. You need to be careful then.
    }
}
