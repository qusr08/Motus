using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano, Jacob Braunhut, Matthew Meyrowitz
// Date Created: 9/12/22
// Date Last Editted: 9/30/22

public enum BulletType {
	PLAYER, DEFLECTABLE, DASHABLE, ENEMY, DEFLECTED
}

public class BulletController : MonoBehaviour {
	[SerializeField] private new Rigidbody2D rigidbody2D;
	[Space]
	[SerializeField] private float bulletSpeed;
	[SerializeField] private Vector2 direction;
	[Space]
	[SerializeField] private BulletType bulletType;

	public Vector2 Direction {
		set {
			direction = value;

			// Have the bullet face the direction it is being shot in
			transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90f);
		}

		get {
			return direction;
		}
	}

	private void OnTriggerEnter2D (Collider2D collision) {
		GameObject collisionGameObject = collision.gameObject;

		// Try and get various components off the collision game object
		// This will tell us what object this bullet collided with
		// For example, if the collision game object has a PlayerController component, we know that this bullet has collided with the player
		PlayerController playerController = collisionGameObject.GetComponent<PlayerController>( );
		BulletController bulletController = collisionGameObject.GetComponent<BulletController>( );
		EnemyController enemyController = collisionGameObject.GetComponent<EnemyController>( );

		// If this bullet collides with another bullet, have nothing happen
		if (bulletController != null) {
			return;
		}

		// If this bullet collides with the player ...
		if (playerController != null) {
			// If the bullet type is not PLAYER or DEFLECTED, then it shouldnt make the player lose health or collide with the player
			if (bulletType == BulletType.PLAYER || bulletType == BulletType.DEFLECTED) {
				return;
			}

			// If the bullet is 
			bool hitEnemyBullet = (bulletType == BulletType.ENEMY);
			// If the bullet is DASHABLE and the player is not dashing, the player should take damage
			bool hitDashableBullet = (bulletType == BulletType.DASHABLE && !playerController.IsDashing);
			// If the bullet is DEFLECTABLE and the player is not deflecting, the player should take damage
			bool hitDeflectableBullet = (bulletType == BulletType.DEFLECTABLE && !playerController.IsDeflecting);

			// If the bullet hits the player correctly in any way
			// ... have the player take damage
			if (hitEnemyBullet || hitDashableBullet || hitDeflectableBullet) {
				playerController.TakeDamage(1);

				Debug.Log("Player was hit by " + bulletType.ToString( ) + " bullet!");
			}

			// If the player is deflecting when colliding with a deflectable bullet
			// ... have the bullet reverse directions
			if (bulletType == BulletType.DEFLECTABLE && playerController.IsDeflecting) {
				bulletType = BulletType.DEFLECTED;
				Direction *= -1;

				return;
			}
		}

		// If this bullet collides with an enemy ...
		if (enemyController != null) {
			// If the bullet type is not PLAYER or DEFLECTED
			// ... then an enemy shot the bullet and it should not collide with any other enemies
			if (bulletType != BulletType.PLAYER && bulletType != BulletType.DEFLECTED) {
				return;
			}

			// If the bullet is PLAYER then have the enemy take damage
			enemyController.TakeDamage(1);
		}

		// Destroy the bullet
		Destroy(gameObject);
	}

	private void FixedUpdate ( ) {
		// If the direction of the bullet has not been set, then do not update the position or decrease the lifetime
		// Each time a bullet is instantiated this direction needs to be set
		if (Direction.magnitude == 0) {
			return;
		}

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

		// Set the color of the bullet depending on the type
		if (bulletType == BulletType.DASHABLE) {
			bullet.GetComponent<Renderer>( ).material.color = Color.blue;
		} else if (bulletType == BulletType.DEFLECTABLE || bulletType == BulletType.DEFLECTED) {
			bullet.GetComponent<Renderer>( ).material.color = Color.green;
		} else if (bulletType == BulletType.ENEMY) {
			bullet.GetComponent<Renderer>( ).material.color = Color.red;
		} else if (bulletType == BulletType.PLAYER) {
			bullet.GetComponent<Renderer>( ).material.color = Color.white;
		}
	}

	// Picks out a random bullet
	/*
	public static BulletType Pick ( ) {
		BulletType pickedType = (BulletType) Random.Range(1, 4);

		return pickedType;
	}
	*/
}
