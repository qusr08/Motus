using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano, Michael Xie, Jacob Braunhut, Steven Feldman
// Date Created:		09/16/22
// Date Last Editted:	10/09/22

public class EnemyController : EntityController {
	[Space]
	[SerializeField] public PlayerController Player;
	[Space]
	[Tooltip("The range that the player needs to be in to start the enemy events in 'playerInSight'.")]
	[SerializeField] public float Sight;
	[Tooltip("The range that the player needs to be in to start the enemy events in 'playerInRange'.")]
	[SerializeField] public float Range;
	[Space]
	[Tooltip("Run these events in order while the player is in sight of this enemy.")]
	[SerializeField] private List<EnemyEvent> playerInSight;
	[Tooltip("Run these events in order while the player is in range of this enemy, or when the enemy gets a certain distance away from the player.")]
	[SerializeField] private List<EnemyEvent> playerInRange;
	[Tooltip("Run these events in order while the player is in sight or in range of the enemy.")]
	[SerializeField] private List<EnemyEvent> regularAttack;
	[Tooltip("Run these events in order after a certain amount of time. Make sure the first element in this array is a delay event with a specified amount of seconds that it should take to start the special attack.")]
	[SerializeField] private List<EnemyEvent> specialAttack;

	private int playerInSightEventIndex = 0;
	private int playerInRangeEventIndex = 0;
	private int regularAttackEventIndex = 0;
	private int specialAttackEventIndex = 0;

	private bool isRunningPlayerInSightEvent = false;
	private bool isRunningPlayerInRangeEvent = false;
	private bool isRunningRegularAttackEvent = false;
	private bool isRunningSpecialAttackEvent = false;

	/// <summary>
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	private new void OnValidate ( ) {
		base.OnValidate( );

		Player = FindObjectOfType<PlayerController>( );
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

		// While the player is not equal to null
		if (Player != null) {
			// Update the enemy aim direction
			Aim = (Player.transform.position - transform.position).normalized;
			AimAngleDegrees = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x);

			/// TODO: REPLACE THIS WITH AI MOVEMENT IN PLAYERINSIGHTT ENEMY EVENT LIST
			Movement = (Player.transform.position - transform.position).normalized;
		} else {
			Movement = Vector2.zero;
		} 

		// Update all enemy event lists
		UpdateEnemyEventList(playerInSight, playerInSightEventIndex, isRunningPlayerInSightEvent, out playerInSightEventIndex, out isRunningPlayerInSightEvent);
		UpdateEnemyEventList(playerInRange, playerInRangeEventIndex, isRunningPlayerInRangeEvent, out playerInRangeEventIndex, out isRunningPlayerInRangeEvent);
		UpdateEnemyEventList(regularAttack, regularAttackEventIndex, isRunningRegularAttackEvent, out regularAttackEventIndex, out isRunningRegularAttackEvent);
		UpdateEnemyEventList(specialAttack, specialAttackEventIndex, isRunningSpecialAttackEvent, out specialAttackEventIndex, out isRunningSpecialAttackEvent);
	}

	/// <summary>
	/// Update the current enemy event in a certain event list.
	/// </summary>
	/// <param name="enemyEvents">The enemy event list to update.</param>
	/// <param name="eventIndex">The index of the current enemy event being updated.</param>
	/// <param name="isRunningEvent">Whether or not an event from this array is currently being updated.</param>
	/// <param name="_eventIndex">The output value of the event index after this method finishes.</param>
	/// <param name="_isRunningEvent">The output value of whether or not an event is currently being run.</param>
	private void UpdateEnemyEventList (List<EnemyEvent> enemyEvents, int eventIndex, bool isRunningEvent, out int _eventIndex, out bool _isRunningEvent) {
		// Both the event index and whether or not an event is running are passed in for this particular array
		// These variables are set like this so different variables can be input and output from the function for different lists of enemy events
		_eventIndex = eventIndex;
		_isRunningEvent = isRunningEvent;

		// If an event is currently running
		// ... update it
		if (_isRunningEvent) {
			enemyEvents[_eventIndex].Execute(gameManager, this);
		}

		while (_eventIndex < enemyEvents.Count) {
			if (!_isRunningEvent) {
				// Start the next enemy event in the attack list
				enemyEvents[_eventIndex].Initialize(gameManager, this);
				_isRunningEvent = true;
			}

			// If the attack is already finished (as in it didn't need to update over time)
			// ... continue to the next event
			if (enemyEvents[_eventIndex].IsFinished) {
				_isRunningEvent = false;
				_eventIndex++;
			} else {
				break;
			}
		}

		// If the last event has been completed
		// ... reset the index back to the beginning of the list so it can restart the next loop
		if (_eventIndex == enemyEvents.Count) {
			_eventIndex = 0;
		}
	}
}