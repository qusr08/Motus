using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano, Michael Xie, Jacob Braunhut, Steven Feldman
// Date Created:		09/16/22
// Date Last Editted:	10/08/22

public class EnemyController : Entity {
	[Space]
	[SerializeField] private PlayerController player;
	[Space]
	[SerializeField] private List<BulletPattern> bulletPatterns;
	[SerializeField] [Min(0f)] private float minTimeBetweenBulletPatterns = 1f;
	[SerializeField] [Min(0f)] private float maxTimeBetweenBulletPatterns = 1f;

	private int currentPatternIndex = 0;
	private float bulletPatternTimer = 0f;

	/// <summary>
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	private new void OnValidate ( ) {
		base.OnValidate( );

		player = FindObjectOfType<PlayerController>( );
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
		if (player != null) {
			// Update the enemy aim direction
			Aim = (player.transform.position - transform.position).normalized;
			AimAngleDegrees = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x);

			// Once the bullet timer has reached 0
			// ... shoot the next bullet pattern
			bulletPatternTimer -= Time.deltaTime;
			if (bulletPatternTimer <= 0) {
				ShootNextBulletPattern( );
			}

			Movement = (player.transform.position - transform.position).normalized;
		} else {
			Movement = Vector2.zero;
		}
	}

	/// <summary>
	/// Shoot the next bullet pattern.
	/// </summary>
	public void ShootNextBulletPattern () {
		// For each bullet instruction of the bullet pattern
		// ... spawn a bullet with the specified values
		foreach(BulletInstruction bulletInstruction in bulletPatterns[currentPatternIndex].bulletInstructions) {
			BulletController.SpawnBullet(bulletPrefab, transform.position, AimAngleDegrees + bulletInstruction.BulletAngleOffsetDegrees, bulletInstruction.BulletSpeed, bulletInstruction.BulletType);
		}

		// Increment the pattern index so the next time the enemy shoots it will shoot the next bullet pattern
		currentPatternIndex++;
		// Make sure the index never goes above the pattern length
		if (currentPatternIndex == bulletPatterns.Count) {
			currentPatternIndex = 0;
		}

		// Reset the bullet timer so there is a delay before the next bullet pattern is fired
		// If you so choose the delay can be random between two values, but you can just set the same value for both the min and the max to achieve a constant bullet fire time
		bulletPatternTimer = Random.Range(minTimeBetweenBulletPatterns, maxTimeBetweenBulletPatterns);
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

// In unity editor:
//	list of enemy ai behaviors (attract, orbit, etc.)
//	list of regular attack bullet patterns
//	list for special attack behaviors