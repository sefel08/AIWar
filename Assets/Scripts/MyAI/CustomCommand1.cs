using System.Collections.Generic;
using UnityEngine;

public class CustomCommand1 : Command<CustomUnit1>, ICommand
{
    CustomUnit1 TestUnit;

    public override void Start()
    {
        TestUnit = units[0];
    }
    public override void Update()
    {
        foreach (var unit in units.Values)
        {
            UnitInfo info = unit.Info;

            if (info.isAlive == false) continue;

            List<EnemyData> visibleEnemies = unit.GetVisibleEnemies();
            if (visibleEnemies.Count > 0)
                Debug.Log("Cone: " + visibleEnemies[0].seenConeAngle);

            Vector2 moveDirection = ((Input.GetAxis("Horizontal") * Vector2.right) +
                                    (Input.GetAxis("Vertical") * Vector2.up)).normalized;

            if (moveDirection != Vector2.zero) unit.Move(moveDirection);

            if(Input.GetKeyDown(KeyCode.G)) unit.Move(new Vector2(1, 1));

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
