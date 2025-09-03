using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomCommand2 : Command
{
    public CustomCommand2(int teamId, GameMap gameMap) : base(teamId, gameMap) { }

    public override void Start()
    {

    }
    public override void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //get random unit from this command and shoot
            if (units.Count > 0)
            {
                int randomIndex = Random.Range(0, units.Count);
                Unit randomUnit = units.Values.ElementAt(randomIndex);
                randomUnit.Shoot();
            }
        }
    }
    public override void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection)
    {

    }
}
