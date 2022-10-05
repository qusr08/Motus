using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano, Jacob Braunhut, Matthew Meyrowitz
// Date Created:		09/12/22
// Date Last Editted:	10/05/22

public enum BulletType {
	PLAYER, DEFLECTABLE, DASHABLE, ENEMY
}

public class BulletController : MonoBehaviour {
	[Tooltip("The sprites for each Bullet Type.\n\nThese sprites should line up with the indeces of the BulletType enum.")]
	[SerializeField]  private Sprite[ ] bulletSprites;
	[Tooltip("The colorblind sprites for each Bullet Type.\n\nThese sprites should line up with the indeces of the BulletType enum.")]
	[SerializeField] private Sprite[ ] colorblindBulletSprites;
	[Space]
	[SerializeField] private Rigidbody2D rigidBody2D;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[Space]
	[SerializeField] private float bulletSpeed;
	[SerializeField] private BulletType bulletType;
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
		PlayerController playerController = collisionGameObject.GetComponent<PlayerController>( );
		BulletController bulletController = collisionGameObject.GetComponent<BulletController>( );
		EnemyController enemyController = collisionGameObject.GetComponent<EnemyController>( );

		// If this bullet collides with another bullet, have nothing happen
		if (bulletController != null) {
			return;
		}

		// If this bullet collides with the player ...
		if (playerController != null) {
			// If the bullet type is not PLAYER
			// ... it shouldnt make the player lose health or collide with the player
			if (BulletType == BulletType.PLAYER) {
				return;
			}

			// If the bullet is from an enemy, then it should harm the player no matter what
			bool hitEnemyBullet = (BulletType == BulletType.ENEMY);
			// If the bullet is DASHABLE and the player is not dashing, the player should take damage
			bool hitDashableBullet = (BulletType == BulletType.DASHABLE && !playerController.IsDashing);
			// If the bullet is DEFLECTABLE and the player is not deflecting, the player should take damage
			bool hitDeflectableBullet = (BulletType == BulletType.DEFLECTABLE && !playerController.IsDeflecting);

			// If the bullet hits the player correctly in any way
			// ... have the player take damage
			if (hitEnemyBullet || hitDashableBullet || hitDeflectableBullet) {
				playerController.Damage(1);
			}

			// If the player is deflecting when colliding with a deflectable bullet
			// ... have the bullet reverse directions
			if (BulletType == BulletType.DEFLECTABLE && playerController.IsDeflecting) {
				BulletType = BulletType.PLAYER;
				Direction *= -1;

				return;
			}

			// Reloads gun when dashing into the bullet
			if (BulletType == BulletType.DASHABLE && playerController.IsDashing) {
				playerController.CurrentAmmo++;
			}
		}

		// If this bullet collides with an enemy ...
		if (enemyController != null) {
			// If the bullet type is not PLAYER
			// ... then an enemy shot the bullet and it should not collide with any other enemies
			if (BulletType != BulletType.PLAYER) {
				return;
			}

			// If the bullet is PLAYER then have the enemy take damage
			enemyController.Damage(1);
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
		rigidBody2D.velocity = bulletSpeed * Time.deltaTime * Direction;
	}

	/// <summary>
	/// Spawn a bullet that moves in a certain direction from a starting point.
	/// </summary>
	/// <param name="bulletPrefab">The bullet prefab Unity object.</param>
	/// <param name="position">The position to start the bullet from.</param>
	/// <param name="direction">The direction the bullet should travel.</param>
	/// <param name="bulletType">The type of bullet to shoot.</param>
	public static void SpawnBullet (GameObject bulletPrefab, Vector2 position, Vector2 direction, BulletType bulletType) {
		// Spawn a bullet at the location of the player
		BulletController bullet = Instantiate(bulletPrefab, position, Quaternion.identity).GetComponent<BulletController>( );

		// Set the direction of the bullet to the direction the player is currently aiming
		bullet.Direction = direction;
		bullet.BulletType = bulletType;

		// Since all of the variables of the bullet have been set, it can be initialized
		// This flag means that the bullet can now function properly
		bullet.IsInitialized = true;
	}
}
