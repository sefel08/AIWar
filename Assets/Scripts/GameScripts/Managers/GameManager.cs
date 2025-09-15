using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [Header("UI properties")]
    [SerializeField] UIDocument uiDocument;
    [Header("Debug")]
    [Header("Static Variables")]
    [SerializeField] int debugInt1;
    [SerializeField] int debugInt2;
    [SerializeField] float debugFloat1;
    [SerializeField] float debugFloat2;
    [SerializeField] bool test1;
    [SerializeField] bool test2;
    [Header("Debug tools")]
    public bool showUnitsFieldOfView;
    public bool showEnemiesInSight;
    public bool showEnemiesInSightBounds;
    public bool showEnemiesInSightBestShootingPoint;
    public bool showShootingRays;
    public bool showUnitRays;
    [Min(0f)]
    public float unitRayDuration; // how long debug unit rays will be visible in seconds
    [Min(0f)]
    public float shootingRayDuration; // how long debug shooting rays will be visible in seconds
    [Header("Prefabs")]
    [SerializeField] GameObject unitPrefab;
    public GameObject projectilePrefab;
    public GameObject hitPrefab;
    public GameObject bloodPrefab;
    [SerializeField] GameObject zonePrefab;
    [Header("Materials")]
    [SerializeField] private Material unlitMaterial;
    [SerializeField] private Material zoneMaterial;
    [Space(10)]
    [Header("Game Settings")]
    [Header("Commands Settings")]
    [SerializeField] int command1UnitCount; // number of units in command 1
    [SerializeField] Gradient command1Color; // color of command 1
    [SerializeField] int command2UnitCount; // number of units in command 2
    [SerializeField] Gradient command2Color; // color of command 2
    [Header("Map Settings")]
    public float MAP_SIZE; // size of the map in units, the map will be a square with this size
    [SerializeField] private float ELEMENTS_SPACING; // how much space will be between the map elements, used to make more spraced out map elements
    [SerializeField] private float MIN_ELEMENT_SIZE; // minimum size of the map element, used to generate the map
    [SerializeField] private float MAX_ELEMENT_SIZE; // maximum size of the map element, used to generate the map
    [SerializeField] private int MAP_PRECISION; // how many tries will be used to generate the map, the higher the value, the more detailed the map will be
    [SerializeField] private Gradient mapColor; // color of the map elements
    [Header("Zone Settings")]
    [SerializeField] private float START_ZONE_SIZE; // initial value of the zone
    [SerializeField] private float ZONE_GROWTH_RATE; // how much the zone will grow each second, used to make the game more dynamic and force players to move towards the center of the map
    public float ZONE_DAMAGE; // how much damage the zone will deal to the units each second, used to make the game more dynamic and force players to move towards the center of the map
    [Header("Unit Settings")]
    public float FIELD_OF_VIEW; // angle of the field of view in degrees, how much the unit can see in front of it
    public float UNIT_MOVE_SPEED; // speed of the unit movement
    public float UNIT_ROTATION_SPEED; // speed of the unit rotation
    public int ENEMY_POINT_PRECISION; // maximum points to test between enemy center and side
    public float SHOOTING_DAMAGE; // damage dealt by the unit when it shoots
    public float SHOOTING_ACCURACY; // accuracy of the shooting, how much the bullet can deviate from the center of the unit in a random direction
    public float SHOOTING_COOLDOWN; // time between shots in seconds
    
    [HideInInspector]
    public float UNIT_SIZE; // half of the unit size, used for raycasting
    [HideInInspector]
    public float zoneSize;
    
    UnitManager unitManager;
    UIManager uiManager;
    CameraManager cameraManager;
    MapGenerator mapGenerator;
    GameMap gameMap;
    public GameMapData gameMapData;

    private GameObject zoneGameObject;

    bool gameStarted = false;
    bool gamePaused = false;
    [HideInInspector]
    public bool gameEnded = false;

    private void OnValidate()
    {
        DebugVariables.testInt1 = debugInt1;
        DebugVariables.testInt2 = debugInt2;
        DebugVariables.testFloat1 = debugFloat1;
        DebugVariables.testFloat2 = debugFloat2;
        DebugVariables.test1 = test1;
        DebugVariables.test2 = test2;
    }

    void Start()
    {
        UNIT_SIZE = unitPrefab.transform.localScale.x / 2f;

        uiManager = new UIManager(uiDocument);
        unitManager = new UnitManager(this, uiManager);
        cameraManager = new CameraManager(unitManager, Camera.main, MAP_SIZE);
        mapGenerator = new MapGenerator(unlitMaterial);

        var data = mapGenerator.GenerateMap(MAP_SIZE, UNIT_SIZE * 2, ELEMENTS_SPACING, MAP_PRECISION, MIN_ELEMENT_SIZE, MAX_ELEMENT_SIZE, mapColor);
        gameMap = data.Item1; //get the game map
        gameMapData = data.Item2; //get the game map data

        InitializeGame();
    }
    private void TogglePauseGame()
    {
        gamePaused = !gamePaused;
        Time.timeScale = gamePaused ? 0f : 1f;
    }
    void Update()
    {
        // starting the game
        if (Input.GetKeyUp(KeyCode.Return) && !gameStarted)
            StartGame();

        // pausing and unpausing the game
        if ((Input.GetKeyDown(KeyCode.Space) && !gamePaused) || Input.GetKeyDown(KeyCode.Escape))
            TogglePauseGame();
        else if (Input.GetKeyUp(KeyCode.Space) && gamePaused)
            TogglePauseGame();

        // updating the game
        if (gameStarted && !gameEnded)
            UpdateGame();

        cameraManager.UpdateCameraPosition();
    }

    private void InitializeGame()
    {
        CreateCommand<CustomUnit2, CustomCommand2>(1, unitPrefab, command1UnitCount, gameMap, new Vector2(0, MAP_SIZE / 2f + 5f), command1Color);
        CreateCommand<CustomUnit2, CustomCommand2>(2, unitPrefab, command2UnitCount, gameMap, new Vector2(0, -MAP_SIZE / 2f - 5f), command2Color);
        
        zoneGameObject = Instantiate(zonePrefab, new Vector3(0f, 0f, -9f), Quaternion.Euler(-90, 0, 0));
        zoneGameObject.transform.localScale = new Vector3(MAP_SIZE * 0.5f, 1, MAP_SIZE * 0.5f);
        zoneSize = START_ZONE_SIZE; //set the initial zone size
        zoneMaterial.SetFloat("_ZoneSize", zoneSize);
    }
    private void StartGame()
    {
        unitManager.StartCommands();
        gameStarted = true;
    }
    private void UpdateGame()
    {
        zoneSize -= ZONE_GROWTH_RATE * Time.deltaTime; //reduce the zone size
        zoneMaterial.SetFloat("_ZoneSize", zoneSize);

        unitManager.UpdateCommands();
        unitManager.RemoveDeadUnits();
    }
    private void CreateCommand<CustomUnit, CustomCommand>(int teamId, GameObject unitPrefab, int numberOfUnits, GameMap map, Vector2 spawnLocation, Gradient teamColor)
        where CustomUnit : Unit<CustomUnit>
        where CustomCommand : Command<CustomUnit>, ICommand
    {
        //create command
        CustomCommand command = (CustomCommand)Activator.CreateInstance(typeof(CustomCommand));
        command.gameMap = new GameMap(map); //give a copy of the map to the command
        CommandData commandData = new CommandData(teamId);

        GameObject parent = new GameObject("Command" + teamId);

        uiManager.CreateTeamContainer(teamId);

        //create units
        for (int unitId = 0; unitId < numberOfUnits; unitId++)
        {
            Color unitColor = teamColor.Evaluate((float)unitId / numberOfUnits);
            Vector2 directionToCenter = (Vector2.zero - spawnLocation).normalized;
            Vector2 spawnPosition = spawnLocation + (new Vector2(spawnLocation.y, -spawnLocation.x) * unitId/100f);
            
            GameObject gameObject = Instantiate(unitPrefab, spawnPosition, Quaternion.Euler(0, 0, Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg), parent.transform);
            gameObject.name = $"Unit({unitId})Team({teamId})";

            Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0;
            rigidbody.freezeRotation = true;
            rigidbody.linearDamping = 10f;
            
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material = unlitMaterial;
            spriteRenderer.color = unitColor;

            CustomUnit unit = (CustomUnit)Activator.CreateInstance(typeof(CustomUnit));
            unit.command = command;

            //set all private fields using reflection
            FieldInfo field = unit.GetType().BaseType.GetField("unitManager", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(unit, unitManager);
            PropertyInfo property = unit.GetType().BaseType.GetProperty("UnitId", BindingFlags.Public | BindingFlags.Instance);
            property?.SetValue(unit, unitId);
            property = unit.GetType().BaseType.GetProperty("TeamId", BindingFlags.Public | BindingFlags.Instance);
            property?.SetValue(unit, teamId);


            UnitData unitData = new UnitData(unit, rigidbody, gameObject);

            //add unit to command
            command.units.Add(unitId, unit);
            //add unit to command data
            commandData.unitDataList.Add(unitId, unitData);
            //add unitData to the unit game objects dictionary
            unitManager.MapGameObjectWithUnitData(gameObject, unitData);

            //add unit to ui
            uiManager.CreateUnitContainer(teamId, unitId, unitId.ToString(), unitColor);
        }

        //add command to the commands dictionary
        unitManager.AddCommand(teamId, command, commandData);
    }
}