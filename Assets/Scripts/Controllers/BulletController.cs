using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano, Jacob Braunhut, Matthew Meyrowitz
// Date Created:		09/12/22
// Date Last Editted:	10/09/22

public enum BulletType {
	PLAYER, DEFLECTABLE, DASHABLE, ENEMY
}

public class BulletController : MonoBehaviour {
	[Tooltip("The sprites for each Bullet Type.\n\nThese sprites should line up with the indeces of the BulletType enum.")]
	[SerializeField] private Sprite[ ] bulletSprites;
	[Tooltip("The colorblind sprites for each Bullet Type.\n\nThese sprites should line up with the indeces of the BulletType enum.")]
	[SerializeField] private Sprite[ ] colorblindBulletSprites;
	[Space]
	[SerializeField] private Rigidbody2D rigidBody2D;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[Space]
	[SerializeField] private BulletType bulletType;
	[SerializeField] public float BulletSpeed;
	[SerializeField] public Vector2 Direction;
	[SerializeField] public bool IsInitialized;

	/// <summary>
	/// The type of this bullet.
	/// </summary>
	public BulletType BulletType {
		set {
			bulletType = value;

			// Set the sprite of the bullet based on the type
			// Since the sprites of the bullets line up with the BulletType enum, it makes this part very simple!
			spriteRenderer.sprite = bulletSprites[(int) bulletType];
			spriteRenderer.sortingOrder = (int) bulletType;
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
		BulletController bulletCollision = collisionGameObject.GetComponent<BulletController>( );
		EnemyController enemyCollision = collisionGameObject.GetComponent<EnemyController>( );

		// If this bullet collides with another bullet, have nothing happen
		if (bulletCollision != null) {
			return;
		}

		// If this bullet collides with the player ...
		if (playerCollision != null) {
			// If the bullet type is not PLAYER
			// ... it shouldnt make the player lose health or collide with the player
			if (BulletType == BulletType.PLAYER) {
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
			}

			// If the player is deflecting when colliding with a deflectable bullet
			// ... have the bullet reverse directions
			if (BulletType == BulletType.DEFLECTABLE && playerCollision.IsDeflecting) {
				BulletType = BulletType.PLAYER;
				Direction *= -1;

				return;
			}

			// Reloads gun when dashing into the bullet
			if (BulletType == BulletType.DASHABLE && playerCollision.IsDashing) {
				playerCollision.CurrentAmmo++;
			}
		}

		// If this bullet collides with an enemy ...
		if (enemyCollision != null) {
			// If the bullet type is not PLAYER
			// ... then an enemy shot the bullet and it should not collide with any other enemies
			if (BulletType != BulletType.PLAYER) {
				return;
			}

			// If the bullet is PLAYER then have the enemy take damage
			enemyCollision.Damage(1);
		}

		// Destroy the bullet if the logic above has allowed it to reach this point
		// This usually means that it has hit something and/or dealt damage
		Destroy(gameObject);
	}

	/// <summary>
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	private void OnValidate ( ) {
		rigidBody2D = GetComponent<Rigidbody2D>( );
		spriteRenderer = GetComponent<SpriteRenderer>( );
	}

	/// <summary>
	/// Called when this bullet is created.
	/// </summary>
	private void Start ( ) {
		OnValidate( );
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
