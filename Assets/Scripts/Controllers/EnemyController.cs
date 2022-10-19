using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano, Michael Xie, Jacob Braunhut, Steven Feldman
// Date Created:		09/16/22
// Date Last Editted:	10/18/22

public class EnemyController : EntityController {
	[Space]
	[SerializeField] public PlayerController PlayerController;
	[Space]
	[Tooltip("The range that the player needs to be in to start the enemy events in 'playerInSight'.")]
	[SerializeField] public float Sight;
	[Tooltip("The range that the player needs to be in to start the enemy events in 'playerInRange'.")]
	[SerializeField] public float Range;
	[Tooltip("The time it takes for the enemy's special attack to be triggered.")]
	[SerializeField] public float SpecialAttackTime;
	[Space]
	[Tooltip("Run these events in order while the player is in sight of this enemy. Contains events that tell the enemy what to do when the player is far away.")]
	[SerializeField] private List<EnemyEvent> playerInSight;
	[Tooltip("Run these events in order while the player is in range of this enemy. Contains events that tell the enemy what to do when the player is close.")]
	[SerializeField] private List<EnemyEvent> playerInRange;
	[Tooltip("Run these events in order while the player is in sight or in range of the enemy.")]
	[SerializeField] private List<EnemyEvent> regularAttack;
	[Tooltip("Run these events in order after a certain amount of time. Make sure the first element in this array is a delay event with a specified amount of seconds that it should take to start the special attack.")]
	[SerializeField] private List<EnemyEvent> specialAttack;

	private float distanceToPlayer = 0;
	private float specialAttackTimer = 0;

	private int playerInSightEventIndex = 0;
	private bool isUpdatingPlayerInSightEvent = false;

	private int playerInRangeEventIndex = 0;
	private bool isUpdatingPlayerInRangeEvent = false;

	private int regularAttackEventIndex = 0;
	private bool isUpdatingRegularAttackEvent = false;

	private int specialAttackEventIndex = 0;
	private bool isUpdatingSpecialAttackEvent = false;
	/// <summary>
	/// Whether or not the enemy is currently doing it's special attack.
	/// </summary>
	public bool IsUpdatingSpecialAttack {
		get {
			return (specialAttackTimer <= 0);
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
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	private new void OnValidate ( ) {
		base.OnValidate( );

		PlayerController = FindObjectOfType<PlayerController>( );
	}

	private new void Start ( ) {
		base.Start( );

		specialAttackTimer = SpecialAttackTime;
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

		// Update the enemy aim direction
		// Might change this later? Enemies should usually just aim at the player
		Aim = (PlayerController.transform.position - transform.position).normalized;
		AimAngleDegrees = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x);

		// Get the distance between the player and this enemy
		distanceToPlayer = (PlayerController.transform.position - transform.position).magnitude;

		// The range needs to be checked before sight because the range will always be smaller than sight.
		// Range should be for interactions with the player at a close range, and sight should be interactions with the player from a far range.
		// If the player is in sight of the enemy
		// ... update event lists
		if (distanceToPlayer <= Sight) {
			// When the player is in sight, decrease the time until the special attack triggers
			specialAttackTimer -= Time.deltaTime;

			// Either update the range event list or sight event list, not both at the same time
			if (distanceToPlayer <= Range) {
				UpdateEventList(playerInRange, ref playerInRangeEventIndex, ref isUpdatingPlayerInRangeEvent);
			} else {
				UpdateEventList(playerInSight, ref playerInSightEventIndex, ref isUpdatingPlayerInSightEvent);
			}

			// The attacking of the enemy should be the same no matter the ai movement
			// Make sure the enemy is not special attacking when updating the regular attack though, as we don't want the two of them to interfere
			if (!IsUpdatingSpecialAttack) {
				/// TODO: MAKE SURE THE ENEMY RESTARTS ITS REGULAR ATTACK EACH TIME IT COMPELTES A SPECIAL ATTACK
				UpdateEventList(regularAttack, ref regularAttackEventIndex, ref isUpdatingRegularAttackEvent);
			}
		} else {
			// If the player is not in range, add a bit of time back to the special attack timer
			specialAttackTimer = Mathf.Clamp(specialAttackTimer + (Time.deltaTime / 2), 0, SpecialAttackTime);
		}

		// If the special attack should be triggered
		// IsUpdatingSpecialAttack automatically gets set to true when the timer is less than or equal to 0
		if (IsUpdatingSpecialAttack) {
			// Update the special attack event list
			UpdateEventList(specialAttack, ref specialAttackEventIndex, ref isUpdatingSpecialAttackEvent);

			// This checks for when the special attack has finished completely
			// When the special attack has finished
			// ... reset the timer and wait for the next time to fire the special attack again
			if (!isUpdatingSpecialAttackEvent && specialAttackEventIndex == 0) {
				specialAttackTimer = SpecialAttackTime;
			}
		}
	}

	/// <summary>
	/// Update the current enemy event in a certain event list.
	/// </summary>
	/// <param name="enemyEvents">The enemy event list to update.</param>
	/// <param name="eventIndex">The index of the current enemy event being updated.</param>
	/// <param name="isRunningEvent">Whether or not an event from this array is currently being updated.</param>
	/// <param name="_eventIndex">The output value of the event index after this method finishes.</param>
	/// <param name="_isRunningEvent">The output value of whether or not an event is currently being run.</param>
	private void UpdateEventList (List<EnemyEvent> enemyEvents, ref int eventIndex, ref bool isRunningEvent) {
		// If an event is currently running
		// ... update it
		if (isRunningEvent) {
			enemyEvents[eventIndex].UpdateEvent(gameManager, this, PlayerController);
		}

		while (eventIndex < enemyEvents.Count) {
			if (!isRunningEvent) {
				// Start the next enemy event in the attack list
				enemyEvents[eventIndex].StartEvent(gameManager, this, PlayerController);
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
		Movement = (position - (Vector2) transform.position).normalized;
	}
}