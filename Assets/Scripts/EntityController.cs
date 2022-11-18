using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/05/22
// Date Last Editted:	10/21/22

public abstract class EntityController : ObjectController {
	// The friction that each entity experiences as they move
	// This will prevent entities from not slowing down if no movement is set
	private float MOVE_FRICTION = 0.9f;

	[Space]
	[SerializeField] [Min(0f)] public float MaxHealth;
	[SerializeField] [Min(0f)] public float MoveSpeed;
	[SerializeField] public bool IsInvincible = false;
	[SerializeField] public bool CanMove = true;
	
	private Color spriteOriginalColor;

	public float CurrentHealth { get; protected set; }
	public Vector2 Movement { get; protected set; }
	public Vector2 Aim { get; protected set; }
	public float AimAngleDegrees { get; protected set; }

	/// <summary>
	/// Whether or not the entity is alive.
	/// </summary>
	public bool IsAlive {
		get {
			return (CurrentHealth > 0);
		}
	}

	/// <summary>
	/// Whether or not the entity is aiming in a direction.
	/// </summary>
	public bool IsAiming {
		get {
			return (Aim.magnitude > 0);
		}
	}

	/// <summary>
	/// Whether or not the entity is moving in a direction.
	/// </summary>
	public bool IsMoving {
		get {
			return (Movement.magnitude > 0);
		}
	}

	/// <summary>
	/// Called when this entity object is created
	/// </summary>
	protected new void Start ( ) {
		base.Start( );

		CurrentHealth = MaxHealth;
		spriteOriginalColor = spriteRenderer.color;

		// If this entity cannot move, make sure you can't move its rigidbody
		rigidBody2D.bodyType = (CanMove ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static);
	}

	/// <summary>
	/// Called at a constant 60 frames per second during game.
	/// </summary>
	protected void FixedUpdate ( ) {
		if (!CanMove) {
			return;
		}

		// Move the entity in the direction they should travel
		if (Movement.magnitude > 0) {
			// Doing kinda some fancy math to make the smoothing right, this can change in the future if a better method is found
			// Mathf.Sqrt() is usually not good to use for loop computations like this, but it works :D
			rigidBody2D.AddForce(Mathf.Sqrt(MoveSpeed) * Time.fixedDeltaTime * Movement, ForceMode2D.Impulse);
			// Make sure the velocity does not go faster than the allowed move speed
			rigidBody2D.velocity = Vector2.ClampMagnitude(rigidBody2D.velocity, MoveSpeed * Time.fixedDeltaTime);
		} else {
			// Slowly decrease the velocity to slow the entity down
			rigidBody2D.velocity *= MOVE_FRICTION;
		}
	}

	/// <summary>
	/// Damages the entity by a certain amount
	/// </summary>
	/// <param name="damage">The damage to deal to the entity.</param>
	/// <returns>The current health of the entity after the damage has been applied.</returns>
	public float Damage (float damage) {
		if (!IsInvincible) {
			// Clamp the health to make sure it does not go below 0 or above the maximum health of the entity
			CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

			StopAllCoroutines( );
			StartCoroutine(FlashColor(Color.red));
		}

		return CurrentHealth;
	}

	/// <summary>
	/// Heals the entity by a certain amount.
	/// </summary>
	/// <param name="health">The health to add to the entity.</param>
	/// <returns>The current health of the entity after the health has been applied.</returns>
	public float Heal (float health) {
		// Clamp the health to make sure it does not go below 0 or above the maximum health of the entity
		CurrentHealth = Mathf.Clamp(CurrentHealth + health, 0, MaxHealth);

		StopAllCoroutines( );
		StartCoroutine(FlashColor(Color.green));

		return CurrentHealth;
	}


	private IEnumerator FlashColor (Color color) {
		spriteRenderer.color = color;

		yield return new WaitForSeconds(0.1f);

		spriteRenderer.color = spriteOriginalColor;

		yield return null;
	}
}
