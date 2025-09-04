using System.Collections.Generic;
using UnityEngine;

public class CustomCommand1 : Command
{
    public CustomCommand1(int teamId, GameMap gameMap) : base(teamId, gameMap) { }

    public override void Start()
    {
        
    }
    public override void Update()
    {
        foreach (CustomUnit1 unit in units.Values)
        {
            UnitInfo info = unit.Info;

            Vector2 moveDirection = ((Input.GetAxis("Horizontal") * Vector2.right) +
                                    (Input.GetAxis("Vertical") * Vector2.up)).normalized;

            if (moveDirection != Vector2.zero) unit.Move(moveDirection);

            //Rotate towards mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 directionToMouse = (mousePosition - info.position).normalized;
            if (directionToMouse != Vector2.zero && Input.GetMouseButton(0)) unit.RotateTowards(directionToMouse);

            if(Input.GetMouseButton(1)) unit.Shoot();
        }
    }
    public override void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection)
    {

    }
}
