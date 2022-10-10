using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/05/22
// Date Last Editted:	10/09/22

public abstract class EntityController : MonoBehaviour {
	[SerializeField] protected GameManager gameManager;
	[SerializeField] protected Rigidbody2D rigidBody2D;
	[SerializeField] protected SpriteRenderer spriteRenderer;
	[Space]
	[SerializeField] [Min(0f)] public float MaxHealth;
	[SerializeField] [Min(0f)] public float MoveSpeed;

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
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	protected void OnValidate ( ) {
		// Get references to these components/gameobjects automatically
		gameManager = FindObjectOfType<GameManager>( );
		rigidBody2D = GetComponent<Rigidbody2D>( );
		spriteRenderer = GetComponent<SpriteRenderer>( );
	}

	/// <summary>
	/// Called when this entity object is created
	/// </summary>
	protected void Start ( ) {
		OnValidate( );

		CurrentHealth = MaxHealth;
	}

	/// <summary>
	/// Called at a constant 60 frames per second during game.
	/// </summary>
	protected void FixedUpdate ( ) {
		// Move the entity in the direction they should travel
		rigidBody2D.velocity = MoveSpeed * Time.fixedDeltaTime * Movement;
	}

	/// <summary>
	/// Damages the entity by a certain amount
	/// </summary>
	/// <param name="damage">The damage to deal to the entity.</param>
	/// <returns>The current health of the entity after the damage has been applied.</returns>
	public float Damage (float damage) {
		// Clamp the health to make sure it does not go below 0 or above the maximum health of the entity
		CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);

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

		return CurrentHealth;
	}
}
