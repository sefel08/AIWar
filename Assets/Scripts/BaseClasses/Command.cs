using System.Collections.Generic;
using UnityEngine;

public abstract class Command
{
    public Command(int teamId, GameMap gameMap)
    {
        this.teamId = teamId;
        this.gameMap = gameMap;
    }

    public readonly int teamId;

    /// <summary>
    /// All units that are part of this command.
    /// </summary>
    public Dictionary<int, Unit> units = new();
    /// <summary>
    /// The collection of map elements which represents walls on the map.
    /// </summary>
    public readonly GameMap gameMap;

    /// <summary>
    /// Method that is called when game starts.
    /// </summary>
    public abstract void Start();
    /// <summary>
    /// Method that is called every frame.
    /// </summary>
    public abstract void Update();
    /// <summary>  
    /// Method that is called when a shot is heard.  
    /// </summary>  
    /// <param name="unitIdsWithDirection">
    /// A dictionary mapping each unit's ID to the direction from which unit heard a shot.
    /// </param>
    public abstract void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection);}
