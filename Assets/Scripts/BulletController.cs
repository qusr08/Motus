using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date: 9/12/22

public enum BulletType {
	PLAYER, DEFLECTABLE, DASHABLE, ENEMY
}

public class BulletController : MonoBehaviour {
	// *** 'lifetime' will probably be replaced with collision down the line, right now it is used to
	// despawn the bullets after a certain amount of time
	[SerializeField] private float lifetime;
	[SerializeField] private float bulletSpeed;
	[Space]
	[SerializeField] private BulletType bulletType;

	public Vector2 Direction;

	private void OnCollisionEnter2D (Collision2D collision) {
		GameObject collisionGameObject = collision.gameObject;

		// If player
		// If the bullet is DASHABLE and player IsDashing
		// ... destroy self
		// else
		// ... decrease health

		// If enemy
		// ... decrease health

		// If not bullet
		// ... destroy self
	}

	private void Update ( ) {
		// If the direction of the bullet has not been set, then do not update the position or decrease the lifetime
		// Each time a bullet is instantiated this direction needs to be set
		if (Direction.magnitude == 0) {
			return;
		}

		// Move the position of the bullet
		transform.position += (Vector3) (Direction * bulletSpeed * Time.deltaTime);

		// Decrease the lifetime by how many seconds has passed
		lifetime -= Time.deltaTime;
		// If the lifetime has reached 0
		// ... destroy the bullet
		if (lifetime <= 0) {
			Destroy(gameObject);
		}
	}

	// Spawn a bullet
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
