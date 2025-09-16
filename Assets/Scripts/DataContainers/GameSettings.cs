using System;
using UnityEngine;

public class GameSettings
{
    public float MAP_SIZE; // size of the map, diameter of the map
    public float ELEMENTS_SPACING; // how much more space will be between the map elements,
    public float MIN_ELEMENT_SIZE; // minimum size of the map element, used to generate the map
    public float MAX_ELEMENT_SIZE; // maximum size of the map element, used to generate the map
    public float ZONE_DAMAGE; // how much damage the zone will deal to the units each second, used to make the game more dynamic and force players to move towards the center of the map
    public float FIELD_OF_VIEW; // angle of the field of view in degrees, how much the unit can see in front of it
    public float UNIT_MOVE_SPEED; // speed of the unit movement
    public float UNIT_ROTATION_SPEED; // speed of the unit rotation
    public float SHOOTING_DAMAGE; // damage dealt by the unit when it shoots
    public float SHOOTING_ACCURACY; // accuracy of the shooting, how much the bullet can deviate from the center of the unit in a random direction
    public float SHOOTING_COOLDOWN; // time between shots in seconds
    public float UNIT_SIZE; // half of the unit size, used for raycasting
}