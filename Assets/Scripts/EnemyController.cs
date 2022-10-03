using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano, Michael Xie, Jacob Braunhut
// Date Created: 9/16/22
// Date Last Editted: 9/30/22

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

	protected Vector2 acceleration = Vector2.zero;
	protected Vector2 desiredVelocity = Vector2.zero;
	protected Vector2 steeringForce = Vector2.zero;
	protected Vector2 ultimateForce = Vector2.zero;

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
					SpreadFire(5, 90f);
					// rngBullShit( );
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
		if (player != null) {
			ultimateForce += Seek(player.position);
		} else {
			ultimateForce += Wander(1f, 3f);
		}
		
		ApplyForce(ultimateForce);

		rigidbody2D.velocity += Time.fixedDeltaTime * acceleration;
		rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, moveSpeed);

		acceleration = Vector2.zero;
	}

	// Have the player lose health
	// int damage: The amount of health to make the player lose
	public void TakeDamage (int damage) {
		health -= damage;

		// Introduce a shine() method to show when the enemy takes damage
	}

	
	// Apply the direction the enemy is going to calculate the acceleration towards that direction
	public void ApplyForce (Vector2 force) {
		acceleration += force;
	}

	// Used to do abstraction for enemies later in the development
	// public abstract void CalcSteeringForces();

	// Point the enemy towards a target position
	protected Vector2 Seek (Vector2 targetPos) {
		//Find the direction for the enemy to point towards
		desiredVelocity = targetPos - (Vector2)transform.position;

		//Normalize the vector
		desiredVelocity = desiredVelocity.normalized * moveSpeed;

		//Calculate the vector for the enemy to steer towards
		steeringForce = desiredVelocity - rigidbody2D.velocity;

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
		return (Vector2) transform.position + (rigidbody2D.velocity * futureTime);
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
	
	// Fires multiple bullets at once
	// int numBullets: The number of bullets to fire
	// float angleSpread: The cone that all the bullets will be encompassed in when firing
	public void SpreadFire (int numBullets, float angleSpread) {
		// The number of bullets entered should not be 1 or less
		if (numBullets <= 1) {
			Debug.LogError("Invalid value for numBullets: " + numBullets);
			return;
		}

		// Calculate the angle to the player
		float playerAngle = Mathf.Rad2Deg * Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x);
		// Calculate the angle gap between the bullets
		float angleGap = angleSpread / (numBullets - 1);
		// Calculate the starting angle for the bullet spread
		float startingAngle = playerAngle - (angleSpread / 2);

		for (float i = 0; i < numBullets; i++) {
			// Calculate the angle to shoot the bullet at
			float angle = Mathf.Deg2Rad * (startingAngle + (angleGap * i));
			Vector2 bulletDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

			// Spawn a bullet that goes in the specified direction
			BulletController.SpawnBullet(bulletPrefab, transform.position, bulletDirection, (BulletType) Random.Range(1, 4));
		}
	}

	/*
	public void rngBullShit ( ) {
		BulletController.SpawnBullet(bulletPrefab, transform.position, (player.position - transform.position).normalized, BulletController.Pick( ));
	}
	*/
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