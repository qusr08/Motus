using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

// Editors:				Frank Alfano, Michael Xie
// Date Created:		09/16/22
// Date Last Editted:	10/23/22

public class EnemyController : EntityController {
	[Space]
	[SerializeField] public PlayerController PlayerController;
	//[SerializeField] public GameController gameController;
	[Space]
	[Tooltip("Run these events in order to tell the enemy how to move.")]
	[SerializeField] private List<EnemyEvent> enemyAI;
	[Tooltip("Run these events in order while the player is in sight or in range of the enemy.")]
	[SerializeField] private List<EnemyEvent> regularAttack;
	[Tooltip("Run these events in order after a certain amount of time. Make sure the first element in this array is a delay event with a specified amount of seconds that it should take to start the special attack.")]
	[SerializeField] private List<EnemyEvent> specialAttack;
	[Tooltip("The time it takes for the enemy's special attack to be triggered.")]
	[SerializeField] public float SpecialAttackTime;
	[Tooltip("The range at which the enemy can 'see' the player. The enemy will not shoot or move if the distance to the player is greater than this amount.")]
	[SerializeField] public float SightRange;
	[SerializeField] private GameObject HealthPrefab;
	[Space]
	[SerializeField] public bool IsMovementHalted;
	[SerializeField] private bool _isJumping;
	[SerializeField] public Sprite shootSprite;
	[SerializeField] public Sprite normalSprite;

	/// <summary>
	/// Whether or not the enemy is jumping.
	/// </summary>
	public bool IsJumping {
		get {
			return _isJumping;
		}

		set {
			_isJumping = value;

			// If the enemy is jumping, disable its collider so it can't get hit by bullets
			objectCollider2D.enabled = !_isJumping;
		}
	}

	private int enemyAIEventIndex = 0;
	private bool isUpdatingEnemyAIEvent = false;

	private int regularAttackEventIndex = 0;
	private bool isUpdatingRegularAttackEvent = false;
	/// <summary>
	/// If the regular attack is currently in the middle of being updated.
	/// </summary>
	public bool IsUpdatingRegularAttackEvents {
		get {
			return (isUpdatingRegularAttackEvent || regularAttackEventIndex != 0);
		}
	}

	private float specialAttackTimer = 0;
	private int specialAttackEventIndex = 0;
	private bool isUpdatingSpecialAttackEvent = false;
	/// <summary>
	/// If the special attack is currently in the middle of being updated.
	/// </summary>
	public bool IsUpdatingSpecialAttackEvents {
		get {
			return (isUpdatingSpecialAttackEvent || specialAttackEventIndex != 0);
		}
	}

	/// <summary>
	/// The angle in radians of the enemy around the player. The angle will follow the unit circle.
	/// </summary>
	public float AngleRadiansAroundPlayer {
		get {
			return Mathf.Atan2(transform.position.y - PlayerController.transform.position.y, transform.position.x - PlayerController.transform.position.x);
		}
	}
	/// <summary>
	/// The distance from the player to this enemy.
	/// </summary>
	public float DistanceToPlayer {
		get {
			return (PlayerController.transform.position - transform.position).magnitude;
		}
	}
	/// <summary>
	/// A value that can be used for modifying specific enemies in events.
	/// </summary>
	public int ModValue { get; private set; }

	/// <summary>
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	private new void OnValidate ( ) {
		base.OnValidate( );

		PlayerController = FindObjectOfType<PlayerController>( );
		//gameController = FindObjectOfType<gameController>();
	}

	/// <summary>
	/// Called when this enemy object is created
	/// </summary>
	private new void Start ( ) {
		OnValidate( );

		base.Start( );

		CloneEnemyEventScriptableObjects(ref enemyAI);
		CloneEnemyEventScriptableObjects(ref regularAttack);
		CloneEnemyEventScriptableObjects(ref specialAttack);

		specialAttackTimer = SpecialAttackTime;

		ModValue = Random.Range(0, 2);
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	/// <summary>
	/// Called as fast as possible as the game is running.
	/// </summary>
	private new void Update ( ) {
		base.Update( );

		// If the enemy is no longer alive
		// ... destroy the game object (FOR NOW)
		if (!IsAlive) {
			//get random value for health drop
			int healthRan = Random.Range(0, 101);


			if (healthRan <= 50) {
				SpawnHealth(transform.position);
			}

			Destroy(gameObject);


			// DEBUG STATS
			gameController.EnemiesDefeated++;

			return;
		}

		// Update the enemy aim direction
		// Might change this later? Enemies should usually just aim at the player
		Aim = (PlayerController.transform.position - transform.position).normalized;
		AimAngleDegrees = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x);

		// If the player is not in sight of the enemy
		// ... do not move or have the enemy shoot
		if (DistanceToPlayer > SightRange) {
			return;
		}

		// Update event lists
		// The range needs to be checked before sight because the range will always be smaller than sight.
		// Range should be for interactions with the player at a close range, and sight should be interactions with the player from a far range.
		// Could get a little weird of ai ranges overlap, so make sure they do not when creating a new enemy
		UpdateEventList(enemyAI, ref enemyAIEventIndex, ref isUpdatingEnemyAIEvent);

		// Only either update the special attack events or the regular attack events, never both at the same time
		if (!IsUpdatingSpecialAttackEvents) {
			UpdateEventList(regularAttack, ref regularAttackEventIndex, ref isUpdatingRegularAttackEvent);
            ChangeSpriteNormal();
        }

		// If it is time for the special attack of the enemy
		// and the regular attack is finished updating
		// ... start the special attack
		if (specialAttackTimer <= 0f && !IsUpdatingRegularAttackEvents) {
			UpdateEventList(specialAttack, ref specialAttackEventIndex, ref isUpdatingSpecialAttackEvent);

            ChangeSpriteShoot();

            if (!IsUpdatingSpecialAttackEvents) {
				specialAttackTimer = SpecialAttackTime;
			}
		}

		specialAttackTimer = Mathf.Clamp(specialAttackTimer - Time.deltaTime, 0, SpecialAttackTime);
	}

	private new void FixedUpdate ( ) {
		base.FixedUpdate( );

		// In order to make enemies stop correctly, their movement needs to be set to 0 after each movement frame in the game
		// The player works differently, though, and this does not need to happen for that
		Movement = Vector2.zero;
	}

	/// <summary>
	/// Update the current enemy event in a certain event list.
	/// </summary>
	/// <param name="enemyEvents">The enemy event list to update.</param>
	/// <param name="eventIndex">The index of the current enemy event being updated.</param>
	/// <param name="isRunningEvent">Whether or not an event from this array is currently being updated.</param>
	private void UpdateEventList (List<EnemyEvent> enemyEvents, ref int eventIndex, ref bool isRunningEvent) {
		// If an event is currently running
		// ... update it
		if (isRunningEvent) {
			enemyEvents[eventIndex].UpdateEvent(gameController, this, PlayerController);
		}

		while (eventIndex < enemyEvents.Count) {
			if (!isRunningEvent) {
				// Start the next enemy event in the attack list
				enemyEvents[eventIndex].StartEvent(gameController, this, PlayerController);
				isRunningEvent = true;
			}

			// If the attack is already finished (as in it didn't need to update over time)
			// ... continue to the next event
			if (enemyEvents[eventIndex].IsFinished) {
				isRunningEvent = false;
				eventIndex++;
			} else {
				break;
			}
		}

		// If the last event has been completed
		// ... reset the index back to the beginning of the list so it can restart the next loop
		if (eventIndex == enemyEvents.Count) {
			eventIndex = 0;
		}
	}

	/// <summary>
	/// Move the enemy towards a position.
	/// </summary>
	/// <param name="position">The position to move towards.</param>
	public void SeekPosition (Vector2 position) {
		// If the enemy's movement is currently halted
		// ... don't have it seek out a new position and change its movement until it is no longer halted
		if (!IsMovementHalted && !IsJumping) {
			Movement = (position - (Vector2) transform.position).normalized;
		}
	}

	/// <summary>
	/// Make sure each enemy's event values are separate when the game is running.
	/// </summary>
	/// <param name="eventList"></param>
	private void CloneEnemyEventScriptableObjects (ref List<EnemyEvent> eventList) {
		for (int i = 0; i < eventList.Count; i++) {
			eventList[i] = Instantiate(eventList[i]);
		}
	}

	/// <summary>
	/// Spawn a health pickup
	/// </summary>
	/// <param name="position">position the health pickup will spawn in</param>
	public void SpawnHealth (Vector2 position) {
		Instantiate(HealthPrefab, position, Quaternion.identity).GetComponent<HealthPickup>( );
	}

	/// <summary>
	/// change enemy to their shooting sprites
	/// </summary>
	private void ChangeSpriteShoot()
	{
		if (shootSprite == null) {
			return;
		}

		spriteRenderer.sprite = shootSprite;

    }

	/// <summary>
	/// change enemy to their defalt sprite
	/// </summary>
	private void ChangeSpriteNormal()
	{
		if (normalSprite == null) {
			return;
		}

		spriteRenderer.sprite = normalSprite;
	}
}