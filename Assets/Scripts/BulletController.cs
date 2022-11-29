using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		09/12/22
// Date Last Editted:	11/18/22

public enum BulletType {
	PLAYER, DEFLECTABLE, DASHABLE, ENEMY, CHARGED
}

public class BulletController : ObjectController {
	[Space]
	[SerializeField] private BulletType bulletType;
	[SerializeField] public float BulletSpeed;
	[SerializeField] public Vector2 Direction;
	[SerializeField] public bool IsInitialized;
	[SerializeField] public bool DidCollide = false;
	[SerializeField] public AudioSource source;
	[SerializeField] public AudioSource deflectSFX;

	/// <summary>
	/// The type of this bullet.
	/// </summary>
	public BulletType BulletType {
		set {
			bulletType = value;

			animator.SetInteger("BulletType", (int) bulletType);
		}

		get {
			return bulletType;
		}
	}

	/// <summary>
	/// Detects when this bullet collides with another object.
	/// </summary>
	/// <param name="collision">The object that the bullet collided with.</param>
	private void OnTriggerEnter2D (Collider2D collision) {
		GameObject collisionGameObject = collision.gameObject;

		// Try and get various components off the collision game object
		// This will tell us what object this bullet collided with
		// For example, if the collision game object has a PlayerController component, we know that this bullet has collided with the player
		PlayerController playerCollision = collisionGameObject.GetComponent<PlayerController>( );
		EnemyController enemyCollision = collisionGameObject.GetComponent<EnemyController>( );
		OffscreenEnemyPointer offscreenEnemy = collisionGameObject.GetComponent<OffscreenEnemyPointer>( );

		// Check to see if the collider is a circle collider
		bool IsCircleCollider = collisionGameObject.GetComponent<CircleCollider2D>( );

		if (!DidCollide) {
			if (offscreenEnemy != null)
            {
				return;
            }

			// If this bullet collides with the player ...
			if (playerCollision != null) {
				// If the bullet type is not PLAYER
				// ... it shouldnt make the player lose health or collide with the player
				if (BulletType == BulletType.PLAYER || BulletType == BulletType.CHARGED) {
					return;
				}

				// If the bullet is from an enemy, then it should harm the player no matter what
				bool hitEnemyBullet = (BulletType == BulletType.ENEMY);
				// If the bullet is DASHABLE and the player is not dashing, the player should take damage
				bool hitDashableBullet = (BulletType == BulletType.DASHABLE && !playerCollision.IsDashing);
				// If the bullet is DEFLECTABLE and the player is not deflecting, the player should take damage
				bool hitDeflectableBullet = (BulletType == BulletType.DEFLECTABLE && !playerCollision.IsDeflecting);

				// If the bullet hits the player correctly in any way
				// ... have the player take damage
				if (hitEnemyBullet || hitDashableBullet || hitDeflectableBullet) {
					playerCollision.Damage(1);

					DidCollide = true;
				}

				// If the player is deflecting when colliding with a deflectable bullet
				// ... have the bullet reverse directions
				if (BulletType == BulletType.DEFLECTABLE && playerCollision.IsDeflecting) {
					BulletType = BulletType.PLAYER;
					Direction *= -1;
					deflectSFX.Play();

					// DEBUG STATS
					gameController.BulletsDeflected++;

					return;
				}

				// Reloads gun when dashing into the bullet
				if (BulletType == BulletType.DASHABLE && playerCollision.IsDashing && IsCircleCollider) {
					// DEBUG STATS
					gameController.BulletsDashedThrough++;

					// Increase the player's charged bullet charge
					// 0.0834f is 1/12 but because of decimal errors we need to write it out like this
					playerCollision.ChargedBulletBarController.Percentage += 0.0834f;

					DidCollide = true;
				}
			}

			// If this bullet collides with an enemy ...
			else if (enemyCollision != null) {
				if (BulletType == BulletType.PLAYER) {
					// If the bullet is PLAYER then have the enemy take damage
					enemyCollision.Damage(1);

					// DEBUG STATS
					gameController.BulletsHit++;

					DidCollide = true;
				} else if (BulletType == BulletType.CHARGED) {
					// If the bullet is CHARGED then have the enemy take damage
					enemyCollision.Damage(25);

					// DEBUG STATS
					gameController.ChargedBulletsHit++;

					// DidCollide = true;

					// The charged bullet is going to pierce through enemies
					return;
				} else {
					// If the bullet type is not PLAYER
					// ... then an enemy shot the bullet and it should not collide with any other enemies
					return;
				}
			}
		}

		// Destroy the bullet if the logic above has allowed it to reach this point
		// This usually means that it has hit something and/or dealt damage
		Destroy(gameObject);
	}

	/// <summary>
	/// Called at a max of 60 frames per second.
	/// </summary>
	private void FixedUpdate ( ) {
		// If the bullet has not been initialized
		// ... do not move it
		if (!IsInitialized) {
			return;
		}

		// Move the position of the bullet
		rigidBody2D.velocity = BulletSpeed * Time.deltaTime * Direction;
	}
}
