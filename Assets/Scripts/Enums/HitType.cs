using UnityEngine;

public enum HitType
{
    Friendly,
    Enemy,
    Map,
    None, // This is used when the raycast did not hit anything. HitData will be null.
    OutOfFieldOfView // This is used when the raycast was outside of the unit's field of view. HitData will be null.
}

