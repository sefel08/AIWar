using System.Collections.Generic;
using UnityEngine;

public class Command<TUnit>
{
    private GameManager gameManagerInstance;

    /// <summary>
    /// All units that are part of this command.
    /// </summary>
    public Dictionary<int, TUnit> units = new();
    /// <summary>
    /// The collection of map elements which represents walls on the map.
    /// </summary>
    public GameMap gameMap;
    /// <summary>
    /// The collection of game settings.
    /// </summary>
    public GameSettings gameSettings;

    public float ZoneSize { get { return gameManagerInstance.zoneSize; } }
    
    /// <summary>
    /// Method that is called when game starts.
    /// </summary>
    public virtual void Start() { }
    /// <summary>
    /// Method that is called every frame.
    /// </summary>
    public virtual void Update() { }
    /// <summary>  
    /// Method that is called when a shot is heard.  
    /// </summary>  
    /// <param name="unitIdsWithDirection">
    /// A dictionary mapping each unit's ID to the direction from which unit heard a shot.
    /// </param>
    public virtual void ShotHeard(Dictionary<int, Vector2> unitIdsWithDirection) { }
}
