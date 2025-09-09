using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitManager
{
    Dictionary<int, (ICommand, CommandData)> commands;
    Dictionary<GameObject, UnitData> gameObjectUnitDataDictionary; // maps game objects to units, used to find units by their game object
    //int - command id, int - unit id
    List<(int, int)> unitsToRemove;

    //game variables
    public readonly float UNIT_SIZE;

    //constants
    private const float MOVE_SPEED = 10f;
    private const float TURN_SPEED = 180f;
    private const float PROJECTILE_DISTANCE = 120f; // distance the projectile will travel if it doesn't hit anything

    GameObjectPool<ProjectileScript> projectilePool; // pool for projectiles to reuse them instead of creating new ones
    ParticlePool hitParticlePool;
    ParticlePool bloodParticlePool;

    GameManager gameManager;
    UIManager uiManager;

    public UnitManager(GameManager gameManager, UIManager uiManager)
    {
        commands = new();
        gameObjectUnitDataDictionary = new();
        unitsToRemove = new();

        this.gameManager = gameManager;
        this.uiManager = uiManager;

        Transform particleParent = new GameObject("Particles").transform;

        projectilePool = new GameObjectPool<ProjectileScript>(particleParent, "Projectiles", gameManager.projectilePrefab, true);
        hitParticlePool = new ParticlePool(particleParent, "HitParticles", gameManager.hitPrefab);
        bloodParticlePool = new ParticlePool(particleParent, "BloodParticles", gameManager.bloodPrefab);

        UNIT_SIZE = gameManager.UNIT_SIZE;
    }

    public void AddCommand(int teamId, ICommand command, CommandData commandData)
    {
        commands.Add(teamId, (command, commandData));
    }
    public void MapGameObjectWithUnitData(GameObject gameObject, UnitData unitData)
    {
        gameObjectUnitDataDictionary.Add(gameObject, unitData);
    }

    // Auto methods
    public void StartCommands()
    {
        foreach (var command in commands.Values.Select(pair => pair.Item1))
        {
            command.Start(); //call start method for each command
        }
    }
    public void UpdateCommands()
    {
        foreach (var pair in commands.Values)
        {
            CommandData commandData = pair.Item2;
            foreach (UnitData unitData in commandData.unitDataList.Values)
            {
                unitData.hasMoved = false; //reset hasMoved flag for each unit
                unitData.hasRotated = false; //reset hasRotated flag for each unit
                if (gameManager.showUnitsFieldOfView)
                    DrawUnitFieldOfView(unitData); //draw unit's field of view for debugging
            }

            pair.Item1.Update(); //call update method for each command
        }
    }
    public void DamageUnitsOutOfZone(float zoneSize, float zoneDamage)
    {
        foreach (CommandData commandData in commands.Values.Select(pair => pair.Item2))
        {
            foreach (UnitData unitData in commandData.unitDataList.Values)
            {
                // Add zone damage to the unit if it is inside the zone
                if (Vector2.Distance(unitData.Position, Vector2.zero) > zoneSize)
                {
                    DamageUnit(unitData, zoneDamage * Time.deltaTime); //apply damage to the unit
                }
            }
        }
    }
    public void RemoveDeadUnits()
    {
        foreach (var commandUnitId in unitsToRemove)
        {
            var commandObjects = commands[commandUnitId.Item1];
            ICommand command = commandObjects.Item1;
            CommandData commandData = commandObjects.Item2;

            if (!commandData.unitDataList.TryGetValue(commandUnitId.Item2, out UnitData unitData))
                continue; //unit data not found, because it was already removed. Two or more bullets hit the same unit at the same time

            IUnit unit = unitData.unit;

            //remove unit from UI
            uiManager.RemoveUnitContainer(unit.TeamId, unit.UnitId);

            //run unit's death function
            unit.OnUnitDeath();

            ////remove unit from the command's units list
            //command.units.Remove(unit.UnitId);

            //remove unit from the command's unit data list
            commandData.unitDataList.Remove(commandUnitId.Item2);
            //remove unit from the unit game objects dictionary
            gameObjectUnitDataDictionary.Remove(unitData.gameObject);
            //destroy unit's game object
            GameObject.Destroy(unitData.gameObject);
        }
        unitsToRemove.Clear();
    }
    public List<UnitData> GetAllUnitData()
    {
        return commands.Values.Select(pair => pair.Item2).SelectMany(commandData => commandData.unitDataList.Values).ToList();
    }

    // Getters for Unit class
    public UnitInfo GetUnitInfo(IUnit unit)
    {
        if (IsUnitAlive(unit))
        {
            UnitData unitData = GetUnitData(unit);
            return new UnitInfo(unitData, GetUnitZoneDistance(unitData));
        }
        else
            return new UnitInfo(false);
    }
    public Vector2 GetUnitPosition(IUnit unit)
    {
        return GetUnitData(unit).Position;
    }
    public Quaternion GetUnitRotation(IUnit unit)
    {
        return GetUnitData(unit).Rotation;
    }
    public float GetUnitZoneDistance(IUnit unit)
    {
        return gameManager.zoneSize - Vector2.Distance(GetUnitPosition(unit), Vector2.zero);
    }
    public bool GetUnitHasMoved(IUnit unit)
    {
        return GetUnitData(unit).hasMoved;
    }
    public bool GetUnitHasRotated(IUnit unit)
    {
        return GetUnitData(unit).hasRotated;
    }
    public float GetUnitHealth(IUnit unit)
    {
        return GetUnitData(unit).health;
    }
    public bool IsUnitAlive(IUnit unit)
    {
        return commands[unit.TeamId].Item2.unitDataList.TryGetValue(unit.UnitId, out UnitData data);
    }

    // Actions for Unit class
    public bool MoveUnitInDirection(IUnit unit, Vector2 direction)
    {
        if (!IsNormalized(direction)) throw new ArgumentException("Direction must be normalized");

        UnitData unitData = GetUnitData(unit);

        if (unitData.hasMoved) return false;
        unitData.hasMoved = true;

        Vector2 position = unitData.Position;
        Vector2 newPosition = position + (direction.normalized * MOVE_SPEED * Time.fixedDeltaTime);
        unitData.rigidbody.MovePosition(newPosition);
        return true;
    }
    public bool MoveUnitTowards(IUnit unit, Vector2 targetPosition)
    {
        UnitData unitData = GetUnitData(unit);

        if (unitData.hasMoved) return false;
        unitData.hasMoved = true;

        Vector2 position = unitData.Position;
        Vector2 direction = (targetPosition - position).normalized;
        Vector2 newPosition = position + (direction * MOVE_SPEED * Time.fixedDeltaTime);
        unitData.rigidbody.MovePosition(newPosition);
        return true;
    }
    public bool RotateUnitTowards(IUnit unit, Quaternion targetRotation)
    {
        UnitData unitData = GetUnitData(unit);

        if (unitData.hasRotated) return false;
        unitData.hasRotated = true;

        unitData.rigidbody.MoveRotation(Quaternion.RotateTowards(unitData.Rotation, targetRotation, TURN_SPEED * Time.fixedDeltaTime));
        return true;
    }
    public bool RotateUnitTowards(IUnit unit, Vector2 direction)
    {
        if (!IsNormalized(direction)) throw new ArgumentException("Direction must be normalized");

        UnitData unitData = GetUnitData(unit);

        if (unitData.hasRotated) return false;
        unitData.hasRotated = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        unitData.rigidbody.MoveRotation(Quaternion.RotateTowards(unitData.Rotation, targetRotation, TURN_SPEED * Time.fixedDeltaTime));
        return true;
    }
    public bool RotateUnitTowardsPoint(IUnit unit, Vector2 targetPosition)
    {
        UnitData unitData = GetUnitData(unit);

        if (unitData.hasRotated) return false;
        unitData.hasRotated = true;

        Vector2 direction = (targetPosition - unitData.Position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        unitData.rigidbody.MoveRotation(Quaternion.RotateTowards(unitData.Rotation, targetRotation, TURN_SPEED * Time.fixedDeltaTime));
        return true;
    }
    public bool RotateUnit(IUnit unit, bool clockwise)
    {
        UnitData unitData = GetUnitData(unit);

        if (unitData.hasRotated) return false;
        unitData.hasRotated = true;

        float angle = TURN_SPEED * Time.fixedDeltaTime;
        if (clockwise) angle = -angle;

        Quaternion rotation = unitData.Rotation;
        float currentAngle = rotation.eulerAngles.z;
        float targetAngle = currentAngle + angle;

        Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        unitData.rigidbody.MoveRotation(Quaternion.RotateTowards(rotation, targetRotation, Mathf.Abs(angle)));
        return true;
    }
    public List<EnemyData> GetEnemiesInSight(IUnit unit)
    {
        List<CommandData> enemyCommandDatas = commands
            .Where(pair => pair.Key != unit.TeamId) //filter out own team
            .Select(pair => pair.Value) //get command and command data
            .Select(pair => pair.Item2) //get command data
            .ToList();

        List<EnemyData> enemiesInSight = new List<EnemyData>();

        UnitData unitData = GetUnitData(unit);

        foreach (var commandData in enemyCommandDatas)
        {
            foreach (var enemy in commandData.unitDataList.Values)
            {
                Vector2? enemyPosition = GetVisibleEnemyPoint(enemy, enemy.gameObject);
                if (enemyPosition.HasValue) // check if the enemy is visible
                {
                    EnemyData enemyData = new EnemyData(enemy, enemyPosition.Value);
                    enemiesInSight.Add(enemyData);
                }
            }
        }

        return enemiesInSight;


        Vector2? GetVisibleEnemyPoint(UnitData enemyData, GameObject enemyGameObject)
        {
            Vector2 unitPosition = unitData.Position;
            Vector2 enemyPosition = enemyData.Position;
            Vector2 direction = (enemyPosition - unitPosition).normalized;
            Vector2 unitDirection = unitData.Direction;

            Vector2 rightPoint = enemyPosition + new Vector2(-direction.y, direction.x) * (UNIT_SIZE - 0.01f);
            Vector2 leftPoint = enemyPosition + new Vector2(direction.y, -direction.x) * (UNIT_SIZE - 0.01f);

            bool isRightPointVisible = IsPointInUnitFieldOfView(unitPosition, unitDirection, rightPoint) && IsPointVisible(rightPoint);
            bool isLeftPointVisible = IsPointInUnitFieldOfView(unitPosition, unitDirection, leftPoint) && IsPointVisible(leftPoint);
            bool isCenterPointVisible = IsPointInUnitFieldOfView(unitPosition, unitDirection, enemyPosition) && IsPointVisible(enemyPosition);

            if (!isRightPointVisible && !isLeftPointVisible && !isCenterPointVisible) return null; //if none of the points are visible, return null

            if (isCenterPointVisible)
            {
                // if none of the points are visible, move both points farther to the edges
                if (!isRightPointVisible && !isLeftPointVisible)
                {
                    rightPoint = GetPointFartherToCenter(rightPoint);
                    leftPoint = GetPointFartherToCenter(leftPoint);
                }
                else
                // if only one point is visible, move the other point farther to the center
                {
                    if (isRightPointVisible)
                        leftPoint = GetPointFartherToCenter(leftPoint);
                    else rightPoint = GetPointFartherToCenter(rightPoint);
                }
            }
            else
            {
                //if both points are visible, move both points closer to the center
                if (isRightPointVisible && isLeftPointVisible)
                {
                    Vector2 closerRightPoint = GetPointCloserToCenter(rightPoint);
                    Vector2 closerLeftPoint = GetPointCloserToCenter(leftPoint);
                    //check wchich point is closer to the center and move the other one
                    if (Vector2.Distance(closerRightPoint, enemyPosition) < Vector2.Distance(closerLeftPoint, enemyPosition))
                        leftPoint = closerRightPoint;
                    else rightPoint = closerLeftPoint;
                }
                else if (isRightPointVisible)
                {
                    //if only right point is visible, move left point closer to the center
                    leftPoint = GetPointCloserToCenter(rightPoint);
                }
                else
                {
                    //if only left point is visible, move right point closer to the center
                    rightPoint = GetPointCloserToCenter(leftPoint);
                }
            }

            // Debug  
            if (gameManager.showEnemiesInSight)
            {
                Vector2 debugDirection = (enemyPosition - unitPosition).normalized;
                float debugDistance = Vector2.Distance(unitPosition, enemyPosition);
                Debug.DrawLine(unitPosition, unitPosition + (debugDirection * (debugDistance - UNIT_SIZE)), Color.cyan);
            }
            if (gameManager.showEnemiesInSightBounds)
            {
                Debug.DrawLine(unitPosition, rightPoint, Color.yellow);
                Debug.DrawLine(unitPosition, leftPoint, Color.yellow);
            }
            if (gameManager.showEnemiesInSightBestShootingPoint)
                Debug.DrawLine(unitPosition, (leftPoint + rightPoint) / 2, Color.red);

            return (leftPoint + rightPoint) / 2; //return avarage of both points as the best shooting position

            Vector2 GetPointCloserToCenter(Vector2 startPoint)
            {
                Vector2 result = startPoint;

                Vector2 leftPoint = enemyPosition;
                Vector2 rightPoint = startPoint;

                for (int i = 1; i < gameManager.ENEMY_POINT_PRECISION; i++)
                {
                    Vector2 middlePoint = (leftPoint + rightPoint) / 2;
                    //test if the middle point is visible
                    if (IsPointInUnitFieldOfView(unitData, middlePoint) && IsPointVisible(middlePoint))
                    {
                        result = middlePoint; //update the result to the middle point
                        rightPoint = middlePoint; //move the point closer to the center
                    }
                    else
                    {
                        leftPoint = middlePoint; //move the point closer to edge
                    }
                }

                return result;
            }
            Vector2 GetPointFartherToCenter(Vector2 targetPoint)
            {
                Vector2 result = enemyPosition;

                Vector2 leftPoint = enemyPosition;
                Vector2 rightPoint = targetPoint;

                for (int i = 1; i < gameManager.ENEMY_POINT_PRECISION; i++)
                {
                    Vector2 middlePoint = (leftPoint + rightPoint) / 2;
                    //test if the middle point is visible
                    if (IsPointInUnitFieldOfView(unitData, middlePoint) && IsPointVisible(middlePoint))
                    {
                        result = middlePoint; //update the result to the middle point
                        leftPoint = middlePoint; //move the point closer to edge
                    }
                    else
                    {
                        rightPoint = middlePoint; //move the point closer to the center
                    }
                }

                return result;
            }
            bool IsPointVisible(Vector2 point)
            {
                Vector2 direction = (point - unitPosition).normalized;
                RaycastHit2D hit = Physics2D.Raycast(unitPosition + direction * (UNIT_SIZE + 0.01f), direction);
                return hit.collider != null && hit.collider.gameObject == enemyGameObject;
            }
        }
    }
    public void Shoot(IUnit unit)
    {
        UnitData unitData = GetUnitData(unit);

        if (unitData.nextShootTime > Time.time) return; //if unit is not ready to shoot, return
        unitData.nextShootTime = Time.time + gameManager.SHOOTING_COOLDOWN; //set next shoot time

        AlertEnemies(unitData); //alert enemies about the shot

        Vector2 unitPosition = unitData.Position;
        Quaternion unitRotation = unitData.Rotation;

        Vector2 direction = (unitRotation * Vector2.right).normalized;
        Vector2 startPosition = unitPosition + direction * (UNIT_SIZE + 0.01f);
        Vector2 directionWithOffset = (unitRotation * new Vector2(1, Random.Range(-gameManager.SHOOTING_ACCURACY, gameManager.SHOOTING_ACCURACY))).normalized;
        RaycastHit2D hit = Physics2D.Raycast(startPosition, directionWithOffset);

        //debug
        if (gameManager.showShootingRays && hit.collider != null)
            Debug.DrawLine(unitPosition + direction * (UNIT_SIZE + 0.01f), hit.point, Color.yellow, gameManager.shootingRayDuration);

        // Get projectile from the pool
        PoolObject<ProjectileScript> projectilePoolObject = projectilePool.GetObjectFromPool();
        GameObject projectile = projectilePoolObject.gameObject;
        projectile.gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(directionWithOffset.y, directionWithOffset.x) * Mathf.Rad2Deg);
        ProjectileScript projectileScript = projectilePoolObject.component;

        if (hit.collider != null)
        {
            // Init the projectile script with the start position and hit point
            projectileScript.Init(startPosition + direction * 2f, hit.point, projectilePoolObject.ReturnObject);

            //if the hit is on the map, return
            if (!gameObjectUnitDataDictionary.TryGetValue(hit.collider.gameObject, out UnitData enemyData))
            {
                // Instantiate hit smoke
                hitParticlePool.SpawnParticle(hit.point, Quaternion.LookRotation(hit.normal));
                return;
            }

            IUnit enemy = enemyData.unit;

            //Skip if it is a self hit
            if (enemy == unit) return;

            // Instantiate blood
            bloodParticlePool.SpawnParticle(hit.point, Quaternion.identity);
            DamageUnit(enemyData, gameManager.SHOOTING_DAMAGE);

            return;
        }

        // Init the projectile script with the start position and a point far away in the direction of the ray
        projectileScript.Init(startPosition + direction * 2f, startPosition + directionWithOffset * PROJECTILE_DISTANCE, projectilePoolObject.ReturnObject);
    }
    public (HitType, HitData) CastARay(IUnit unit, Vector2 direction)
    {
        UnitData unitData = GetUnitData(unit);

        if (!IsDirectionInUnitFieldOfView(unitData, direction))
            return (HitType.OutOfFieldOfView, null);

        Vector2 unitPosition = unitData.Position;

        RaycastHit2D hit = Physics2D.Raycast(unitPosition + direction * (UNIT_SIZE + 0.01f), direction);

        //debug
        if (gameManager.showUnitRays)
        {
            if (hit.collider == null)
                Debug.DrawLine(unitPosition + direction * (UNIT_SIZE + 0.01f), unitPosition + direction * (UNIT_SIZE + 0.01f) + (direction * gameManager.MAP_SIZE * 3f), Color.magenta, gameManager.unitRayDuration);
            else
                Debug.DrawLine(unitPosition + direction * (UNIT_SIZE + 0.01f), hit.point, Color.magenta, gameManager.unitRayDuration);
        }

        HitType hitType = GetHitType(hit, unit);

        return (hitType, GetHitData(hit, hitType));
    }
    public (HitType, HitData) CastARay(IUnit unit, Vector2 direction, float distance)
    {
        if (distance <= 0) throw new ArgumentException("Distance must be greater than 0");

        UnitData unitData = GetUnitData(unit);

        if (!IsDirectionInUnitFieldOfView(unitData, direction))
            return (HitType.OutOfFieldOfView, null);

        Vector2 unitPosition = unitData.Position;

        RaycastHit2D hit = Physics2D.Raycast(unitPosition + direction * (UNIT_SIZE + 0.01f), direction, distance);

        //debug
        if (gameManager.showUnitRays)
        {
            if (hit.collider == null)
                Debug.DrawLine(unitPosition + direction * (UNIT_SIZE + 0.01f), unitPosition + direction * (UNIT_SIZE + 0.01f) + (direction * distance), Color.magenta, gameManager.unitRayDuration);
            else
                Debug.DrawLine(unitPosition + direction * (UNIT_SIZE + 0.01f), hit.point, Color.magenta, gameManager.unitRayDuration);
        }

        HitType hitType = GetHitType(hit, unit);

        return (hitType, GetHitData(hit, hitType));
    }
    public (HitType, HitData) CastARayTowardsPoint(IUnit unit, Vector2 point, bool useDistance)
    {
        UnitData unitData = GetUnitData(unit);
        Vector2 direction = (point - unitData.Position).normalized;
        if (useDistance)
        {
            float distance = Vector2.Distance(unitData.Position, point);
            return CastARay(unit, direction, distance);
        }
        return CastARay(unit, direction);
    }
    public bool IsPointInUnitFieldOfView(IUnit unit, Vector2 point)
    {
        UnitData unitData = GetUnitData(unit);
        Vector2 direction = (point - unitData.Position).normalized;
        float angle = Vector2.Angle(unitData.Direction, direction);
        return angle < gameManager.FIELD_OF_VIEW / 2;
    }
    public bool IsDirectionInUnitFieldOfView(IUnit unit, Vector2 direction)
    {
        if (!IsNormalized(direction)) throw new ArgumentException("Direction must be normalized");

        UnitData unitData = GetUnitData(unit);
        float angle = Vector2.Angle(unitData.Direction, direction);
        return angle < gameManager.FIELD_OF_VIEW / 2;
    }

    // Helper methods
    private HitType GetHitType(RaycastHit2D hit, IUnit unit)
    {
        if (hit.collider == null) return HitType.None;
        if (!gameObjectUnitDataDictionary.TryGetValue(hit.collider.gameObject, out UnitData enemyData))
            return HitType.Map; //hit on the map
        return enemyData.unit.TeamId == unit.TeamId ? HitType.Friendly : HitType.Enemy; //hit on friendly or enemy unit
    }
    private HitData GetHitData(RaycastHit2D hit, HitType hitType)
    {
        if (hitType == HitType.None) return null;

        int elementId = 0;
        if (hitType == HitType.Map)
        {
            MapElementData mapElementData = gameManager.gameMapData.gameObjectWithElementDatas[hit.collider.gameObject];
            elementId = mapElementData.mapElement.ElementId;
        }

        return new HitData(hit.point, hit.distance, hit.normal, elementId);
    }
    private void AlertEnemies(UnitData shooter)
    {
        foreach (var pair in commands.Values)
        {
            if (pair.Item2.teamId == shooter.unit.TeamId) continue; //skip own team  

            Dictionary<int, Vector2> unitIdsWithDirection = new Dictionary<int, Vector2>();

            foreach (var unitData in pair.Item2.unitDataList.Values)
            {
                Vector2 directionToShooter = (shooter.Position - unitData.Position).normalized;
                float distanceToShooter = Vector2.Distance(unitData.Position, shooter.Position);

                // Add randomness to direction based on distance  
                float randomnessFactor = Mathf.Clamp01(distanceToShooter / (gameManager.MAP_SIZE));
                float easedRandomnessFactor = -(Mathf.Cos(Mathf.PI * randomnessFactor) - 1) / 5; // Easing function to reduce randomness at close distances
                Vector2 randomOffset = new Vector2(Random.Range(-easedRandomnessFactor, easedRandomnessFactor), Random.Range(-easedRandomnessFactor, easedRandomnessFactor));
                Vector2 randomizedDirection = (directionToShooter + randomOffset).normalized;

                unitIdsWithDirection.Add(unitData.unit.UnitId, randomizedDirection);
            }

            // Notify the command about the shot  
            pair.Item1.ShotHeard(unitIdsWithDirection);
        }
    }
    private UnitData GetUnitData(IUnit unit)
    {
        UnitData unitData;

        if(!commands[unit.TeamId].Item2.unitDataList.TryGetValue(unit.UnitId, out unitData))
            throw new Exception("Tried to run unit method on a dead unit");
        
        return unitData;
    }
    private void DamageUnit(UnitData unitData, float damage)
    {
        unitData.health -= damage;

        uiManager.UpdateHealthBar(unitData.unit.TeamId, unitData.unit.UnitId, unitData.health);

        //if unit is dead, add unit to be removed
        if (unitData.health <= 0)
        {
            unitsToRemove.Add((unitData.unit.TeamId, unitData.unit.UnitId));
        }
    }
    private bool IsNormalized(Vector2 direction)
    {
        if(direction == Vector2.zero) return false;
        return Mathf.Abs(direction.magnitude - 1f) < 1e-5f;
    }
    private float GetUnitZoneDistance(UnitData unitData)
    {
        return gameManager.zoneSize - Vector2.Distance(unitData.Position, Vector2.zero);
    }

    //more optimized methods for unit field of view used in GetEnemiesInSight. Gets the unit position faster.
    private bool IsPointInUnitFieldOfView(UnitData unitData, Vector2 point)
    {
        Vector2 direction = (point - unitData.Position).normalized;
        return IsDirectionInUnitFieldOfView(unitData, direction);
    }
    private bool IsDirectionInUnitFieldOfView(UnitData unitData, Vector2 direction)
    {
        float angle = Vector2.Angle(unitData.Direction, direction);
        return angle < gameManager.FIELD_OF_VIEW / 2;
    }
    private bool IsPointInUnitFieldOfView(Vector2 unitPosition, Vector2 unitDirection, Vector2 point)
    {
        Vector2 direction = (point - unitPosition).normalized;
        float angle = Vector2.Angle(unitDirection, direction);
        return angle < gameManager.FIELD_OF_VIEW / 2;
    }

    private void DrawUnitFieldOfView(UnitData unitData)
    {
        Vector2 directionLeft = Quaternion.Euler(0, 0, gameManager.FIELD_OF_VIEW / 2) * unitData.Direction;
        Vector2 directionRight = Quaternion.Euler(0, 0, -gameManager.FIELD_OF_VIEW / 2) * unitData.Direction;
        Debug.DrawLine(unitData.Position, unitData.Position + directionLeft * (gameManager.MAP_SIZE), Color.gray);
        Debug.DrawLine(unitData.Position, unitData.Position + directionRight * (gameManager.MAP_SIZE), Color.gray);
    }
}
