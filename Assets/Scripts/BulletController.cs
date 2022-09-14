using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date Created: 9/12/22
// Date Last Editted: 9/14/22

public enum BulletType {
	PLAYER, DEFLECTABLE, DASHABLE, ENEMY
}

public class BulletController : MonoBehaviour {
	[SerializeField] private new Rigidbody2D rigidbody2D;
	[Space]
	// *** 'lifetime' will probably be replaced with collision down the line, right now it is used to
	// despawn the bullets after a certain amount of time
	[SerializeField] private float lifetime;
	[SerializeField] private float bulletSpeed;
	[Space]
	[SerializeField] private BulletType bulletType;

	public Vector2 Direction;

	private void OnTriggerEnter2D (Collider2D collision) {
		GameObject collisionGameObject = collision.gameObject;

		// Try and get various components off the collision game object
		// This will tell us what object this bullet collided with
		// For example, if the collision game object has a PlayerController component, we know that this bullet has collided with the player
		PlayerController playerController = collisionGameObject.GetComponent<PlayerController>( );
		BulletController bulletController = collisionGameObject.GetComponent<BulletController>( );

		// If this bullet collides with another bullet, have nothing happen
		if (bulletController != null) {
			return;
		}

		// If this bullet collides with the player ...
		if (playerController != null) {
			// If the bullet type is not PLAYER, then it shouldnt make the player lose health or collide with the player
			if (bulletType == BulletType.PLAYER) {
				return;
			}

			// If the bullet is 
			bool hitEnemyBullet = (bulletType == BulletType.ENEMY);
			// If the bullet is DASHABLE and the player is not dashing, the player should take damage
			bool hitDashableBullet = (bulletType == BulletType.DASHABLE && !playerController.IsDashing);
			// TO DO: Add logic for deflectable bullets when those are added in
			bool hitDeflectableBullet = (false);

			// If the bullet hits the player correctly in any way
			// ... have the player take damage
			if (hitEnemyBullet || hitDashableBullet || hitDeflectableBullet) {
				playerController.TakeDamage(1);
			}
		}

		// if EnemyController != null
		// ... if bulletType != PLAYER
		// ... ... return
		// ... damage enemy

		// Destroy the bullet
		Destroy(gameObject);
	}

	private void Update ( ) {
		// If the direction of the bullet has not been set, then do not update the position or decrease the lifetime
		// Each time a bullet is instantiated this direction needs to be set
		if (Direction.magnitude == 0) {
			return;
		}

		// Decrease the lifetime by how many seconds has passed
		lifetime -= Time.deltaTime;
		// If the lifetime has reached 0
		// ... destroy the bullet
		if (lifetime <= 0) {
			Destroy(gameObject);
		}
	}

	private void FixedUpdate ( ) {
		// Move the position of the bullet
		rigidbody2D.velocity = bulletSpeed * Time.deltaTime * Direction;
	}

	// Spawn a bullet that moves in a direction from a starting point
	// GameObject bulletPrefab: The bullet game object
	// Vector2 position: The starting position for the bullet
	// Vector2 direction: The direction for the bullet to move
	// BulletType bulletType: The type of the bullet (see BulletType enum)
	public static void SpawnBullet (GameObject bulletPrefab, Vector2 position, Vector2 direction, BulletType bulletType) {
		// Spawn a bullet at the location of the player
		BulletController bullet = Instantiate(bulletPrefab, position, Quaternion.identity).GetComponent<BulletController>( );
		// Set the direction of the bullet to the direction the player is currently aiming
		bullet.Direction = direction;
		bullet.bulletType = bulletType;
	}
}
