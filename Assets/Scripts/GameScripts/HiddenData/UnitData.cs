using UnityEngine;
using UnityEngine.UIElements;

public class UnitData
{
    public IUnit unit;
    public GameObject gameObject;
    public Rigidbody2D rigidbody;

    public Vector2 Position { get { return rigidbody.position; } }
    public Quaternion Rotation { get { return gameObject.transform.rotation; } }
    public Vector2 Direction { get { return Rotation * Vector2.right; } }

    public bool hasMoved;
    public bool hasRotated;
    public float health;
    public float nextShootTime; // cooldown for shooting
    public bool isMoving;

    public bool isAlive { get { return health > 0; } }
    public bool canShoot { get { return Time.time >= nextShootTime; } }
    public float shootCooldown { get 
        { 
            float cooldown = nextShootTime - Time.time; 
            return cooldown < 0 ? 0 : cooldown; 
        } 
    }

    public UnitData(IUnit unit, Rigidbody2D rigidbody, GameObject gameObject)
    {
        hasMoved = false;
        hasRotated = false;
        isMoving = false;
        health = 100; // default health value
        nextShootTime = 0f; // initialize next shoot time
        this.unit = unit;
        this.rigidbody = rigidbody;
        this.gameObject = gameObject;
    }
}
