using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date Created: 9/16/22
// Date Last Editted: 9/16/22

// *** DUMMY CLASS This is definitely gonna change later just wanted to have some starting point

public class EnemyController : MonoBehaviour {
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform target;
	[SerializeField] private float shootTime;
	[SerializeField] private int health;

    protected Vector3 acceleration = Vector3.zero;
    protected Vector3 velocity = Vector3.forward;
    protected Vector3 desiredVelocity = Vector3.zero;
    protected Vector3 steeringForce = Vector3.zero;
    protected Vector3 UltimateForce = Vector3.zero;

    protected float maxSpeed = 0.01f;
    protected float maxForce = 0.15f;
    protected float mass = 1.0f;

    protected float futureTime = 1f;

    private float timer;

	private bool IsAlive
    {
        get
        {
			return (health > 0);
        }
    }

	private void Start ( ) {
		timer = shootTime;
	}

	private void Update ( ) {
		if (IsAlive)
        {
			if (target != null) {
				// transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(transform.position.y - target.position.y, transform.position.x - target.position.y) + 90f);

				timer -= Time.deltaTime;
				if (timer <= 0) {
                    rngBullShit();
					timer = shootTime;
				}

                UltimateForce += Seek(target.position);
			}
            else
            {
                UltimateForce += Wander(futureTime, 3);
            }

            ApplyForce(UltimateForce);

            velocity += acceleration * Time.deltaTime;

            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

            transform.position += velocity;

            acceleration = Vector3.zero;
        }
        else
        {
			Destroy(gameObject, 2);
        }
	}

	// Have the player lose health
	// int damage: The amount of health to make the player lose
	public void TakeDamage (int damage) {
		// TO DO: Implement this later
		health -= damage;
		//introduce a shine() method to show when the enemy takes damage
	}

    //Apply the direction the enemy is going to calculate the acceleration towards that direction
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    //Used to do abstraction for enemies later in the development
    //public abstract void CalcSteeringForces();

    //Point the enemy towards a target position
    protected Vector3 Seek(Vector3 targetPos)
    {
        //Find the direction for the enemy to point towards
        desiredVelocity = targetPos - transform.position;

        //Normalize the vector
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        //Calculate the vector for the enemy to steer towards
        steeringForce = desiredVelocity - velocity;

        return steeringForce;
    }

    //Run away from the target position
    public Vector3 Flee(Vector3 targetPos)
    {
        desiredVelocity = Seek(targetPos);

        desiredVelocity *= -1;

        return desiredVelocity;
    }

    //Used to calculate the future position of the enemy
    public Vector3 CalculateFuturePosition(float futureTime)
    {
        Vector3 futurePosition = transform.position + velocity * futureTime;

        return futurePosition;
    }

    //Wander around the arena to simulate enemy movement
    public Vector3 Wander(float futuretime, float radius)
    {
        //Calculate the future position of the enemy
        Vector3 futurePos = CalculateFuturePosition(futuretime);

        //Go in a random angle in a radius from the calculated position
        float angle = Random.Range(0, 360);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius + futurePos.x;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius + futurePos.z;

        return (Seek(new Vector3(x, 0, z)));
    }


    //Fires multiple bullets at once.
    public void SpreadFire(int numBullets, float angleSpread)
    {
        float targetAngle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x);

        for (float i = angleSpread/numBullets/-2f; i <= numBullets; i += angleSpread / numBullets)
        {
            Vector2 bulletDirection = new Vector2(Mathf.Cos(i + targetAngle), Mathf.Sin(i + targetAngle));
            BulletController.SpawnBullet(bulletPrefab, transform.position, bulletDirection, BulletType.ENEMY);
        }
    }

    public void rngBullShit()
    {
         BulletController.SpawnBullet(bulletPrefab, transform.position, (target.position - transform.position).normalized, BulletController.Pick());
    }
}

// Bullet pattern planning (PROLLY WILL CHANGE)

// BulletPattern
 
//      {BulletType, Angle, Delay}
        //pattern.Add(BulletType.DASHABLE, 0f, shootTime); Laser
        //pattern.Add(BulletType.???, Current Bullet Angle + 60f, shootTime); 6-way Bullets
        //pattern.Add(BulletType.???, 0f, shootTime); Potential Boomerang
        //
//pattern.Add(BulletType.DASHABLE, 0f, shootTime); Laser
//pattern.Add(BulletType.???, Current Bullet Angle + 60f, shootTime); 6-way Bullets
//pattern.Add(BulletType.???, 0f, shootTime); Potential Boomerang
//

// ShootPattern()
//		for all list items:
//			SpawnBullet(...)