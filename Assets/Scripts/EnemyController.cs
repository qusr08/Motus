using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date Created: 9/16/22
// Date Last Editted: 9/16/22

// *** DUMMY CLASS This is definitely gonna change later just wanted to have some starting point

public class EnemyController : MonoBehaviour {
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private new Rigidbody2D rigidbody2D;
	[SerializeField] private Transform player;
	[Space]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float shootTime;
	[SerializeField] private int health;
	[Space]
	[SerializeField] public Vector2 Movement;

	/*
	protected Vector2 acceleration = Vector2.zero;
	protected Vector2 velocity = Vector2.zero;
	protected Vector2 desiredVelocity = Vector2.zero;
	protected Vector2 steeringForce = Vector2.zero;
	protected Vector2 ultimateForce = Vector2.zero;
	*/

	private float timer;

	private bool IsAlive {
		get {
			return (health > 0);
		}
	}

	private void OnValidate ( ) {
		player = FindObjectOfType<PlayerController>( ).transform;
	}

	private void Start ( ) {
		OnValidate( );

		timer = shootTime;
	}

	private void Update ( ) {
		// While the enemy is alive
		if (IsAlive) {
			// While the player is not equal to null
			if (player != null) {
				// After a certain amount of time, shoot a bullet
				timer -= Time.deltaTime;
				if (timer <= 0) {
					rngBullShit( );
					timer = shootTime;
				}

				// Update the movement direction for the enemy
				Movement = (player.position - transform.position).normalized;
			}
		} else {
			Destroy(gameObject);
		}
	}

	private void FixedUpdate ( ) {
		// Either seek or wander around depending on if the player is not null
		/*
		if (player != null) {
			ultimateForce += Seek(player.position);
		} else {
			ultimateForce += Wander(1f, 3f);
		}
		*/

		// ApplyForce(ultimateForce);

		// Move the enemy
		// velocity += acceleration * Time.deltaTime;
		// velocity = Vector3.ClampMagnitude(velocity, moveSpeed);

		rigidbody2D.velocity = moveSpeed * Time.fixedDeltaTime * Movement;

		// acceleration = Vector3.zero;
	}

	// Have the player lose health
	// int damage: The amount of health to make the player lose
	public void TakeDamage (int damage) {
		health -= damage;

		// Introduce a shine() method to show when the enemy takes damage
	}

	/*
	// Apply the direction the enemy is going to calculate the acceleration towards that direction
	public void ApplyForce (Vector2 force) {
		acceleration += force;
	}

	// Used to do abstraction for enemies later in the development
	// public abstract void CalcSteeringForces();

	// Point the enemy towards a target position
	protected Vector2 Seek (Vector2 targetPos) {
		//Find the direction for the enemy to point towards
		desiredVelocity = targetPos - (Vector2) transform.position;

		//Normalize the vector
		desiredVelocity = desiredVelocity.normalized * moveSpeed;

		//Calculate the vector for the enemy to steer towards
		steeringForce = desiredVelocity - velocity;

		return steeringForce;
	}
		// Run away from the target position
		public Vector2 Flee (Vector2 targetPos) {
			desiredVelocity = -Seek(targetPos);

			desiredVelocity *= -1;

			return desiredVelocity;
		}

		// Used to calculate the future position of the enemy
		public Vector2 CalculateFuturePosition (float futureTime) {
			return (Vector2) transform.position + (velocity * futureTime);
		}

		// Wander around the arena to simulate enemy movement
		public Vector2 Wander (float futuretime, float radius) {
			// Calculate the future position of the enemy
			Vector2 futurePos = CalculateFuturePosition(futuretime);

			// Go in a random angle in a radius from the calculated position
			float angle = Random.Range(0, 360);
			float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius + futurePos.x;
			float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius + futurePos.y;

			return (Seek(new Vector2(x, y)));
		}
	*/


	//Fires multiple bullets at once.
	public void SpreadFire (int numBullets, float angleSpread) {
		float targetAngle = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x);

		for (float i = angleSpread / numBullets / -2f; i <= numBullets; i += angleSpread / numBullets) {
			Vector2 bulletDirection = new Vector2(Mathf.Cos(i + targetAngle), Mathf.Sin(i + targetAngle));
			BulletController.SpawnBullet(bulletPrefab, transform.position, bulletDirection, BulletType.ENEMY);
		}
	}

	public void rngBullShit ( ) {
		BulletController.SpawnBullet(bulletPrefab, transform.position, (player.position - transform.position).normalized, BulletController.Pick( ));
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