using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano, Michael Xie, Jacob Braunhut, Steven Feldman
// Date Created:		09/16/22
// Date Last Editted:	10/05/22

public class EnemyController : Entity {
	[SerializeField] private PlayerController playerController;
	[Space]
	[SerializeField] [Min(0f)] private float shootTime; // TEMP, WILL BE REPLACED WITH AI FUNCTIONALITY
	[SerializeField] private float shootTimer;
	[SerializeField] [Min(0f)] private int range;

	/// <summary>
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	private new void OnValidate ( ) {
		base.OnValidate( );

		playerController = FindObjectOfType<PlayerController>( );
	}

	/// <summary>
	/// Called when this entity object is created
	/// </summary>
	private new void Start ( ) {
		base.Start( );

		shootTimer = shootTime;
	}

	/// <summary>
	/// Called as fast as possible as the game is running.
	/// </summary>
	private void Update ( ) {
		// If the enemy is no longer alive
		// ... destroy the game object (FOR NOW)
		if (!IsAlive) {
			Destroy(gameObject);

			return;
		}

		// While the player is not equal to null
		if (playerController != null) {
			// After a certain amount of time, shoot a bullet
			shootTimer -= Time.deltaTime;
			if (shootTimer <= 0) {
				SpreadFire(5, 90f);
				shootTimer = shootTime;
			}

			Movement = (playerController.transform.position - transform.position).normalized;
		}
	}

	/// <summary>
	/// Shoot a spread of bullets.
	/// </summary>
	/// <param name="numBullets">The number of bullets to shoot. Must be greater than 1.</param>
	/// <param name="angleSpread">The angle length of the arc that the bullets are spread across. Must be greater than 0.</param>
	public void SpreadFire (int numBullets, float angleSpread) {
		// Calculate the angle to the player
		float playerAngle = Mathf.Rad2Deg * Mathf.Atan2(playerController.transform.position.y - transform.position.y, playerController.transform.position.x - transform.position.x);
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
}

// Bullet pattern planning (PROLLY WILL CHANGE)

// BulletPattern

//      {BulletType, Angle, Delay}
//pattern.Add(BulletType.DASHABLE, 0f, shootTime); Laser
//pattern.Add(BulletType.???, Current Bullet Angle + 60f, shootTime); 6-way Bullets
//pattern.Add(BulletType.???, 0f, shootTime); Potential Boomerang
//

// ShootPattern()
//		for all list items:
//			SpawnBullet(...)

// In Enemy Class:
// Make a list for enemy AI
//	Move towards player at certain speed
//	Circle Player
// Add an EnemyEvent Object
//	ai function that will run after specified time
//	  this function will specify the position that the enemy moves to
//	time between each time the ai runs (leave out to have the ai function run every frame)
// Make a list for attacks
//	Dash/jump back
//	Stop moving
//	Bullet pattern
//	** Delay between each item