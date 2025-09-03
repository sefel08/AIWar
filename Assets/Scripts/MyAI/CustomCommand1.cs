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

            if (Input.GetKey(KeyCode.W))
            {
                unit.Move(info.direction); // Move forward
            }
            if (Input.GetKey(KeyCode.S))
            {
                unit.Move(-info.direction); // Move backward
            }
            if (Input.GetKey(KeyCode.A))
            {
                Vector2 leftDirection = new Vector2(-info.direction.y, info.direction.x); // Perpendicular left
                unit.Move(leftDirection);
            }
            if (Input.GetKey(KeyCode.D))
            {
                Vector2 rightDirection = new Vector2(info.direction.y, -info.direction.x); // Perpendicular right
                unit.Move(rightDirection);
            }

            // Add rotation with Q and E
            if (Input.GetKey(KeyCode.Q))
            {
                unit.Rotate(false); // Rotate counterclockwise
            }
            if (Input.GetKey(KeyCode.E))
            {
                unit.Rotate(true); // Rotate clockwise
            }

            // Add shooting with space
            if (Input.GetKey(KeyCode.Space))
            {
                unit.Shoot();
            }

            Debug.Log(unit.IsDirectionInFieldOfView(Vector2.left));
        }
    }
    public override void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection)
    {

    }
}
