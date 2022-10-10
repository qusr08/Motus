using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/09/22
// Date Last Editted:	10/10/22

public abstract class EnemyEvent : ScriptableObject {
	/// <summary>
	/// Whether or not the enemy event is finished.
	/// </summary>
	public bool IsFinished { get; protected set; }

	/// <summary>
	/// Called when the event starts.
	/// 
	/// *** Make sure to set IsFinished in this method.
	/// *** If the enemy event does not need to be updated and is a instant change, set IsFinished to true
	/// *** If the enemy event needs time to be completed, set IsFinished to false and set it to true in Execute().
	/// </summary>
	/// <param name="gameManager">A reference to the game manager object.</param>
	/// <param name="enemyController">A reference to the enemy controller that called this event.</param>
	/// <param name="playerController">A reference to the player controller in the scene.</param>
	public abstract void Initialize (GameManager gameManager, EnemyController enemyController, PlayerController playerController);

	/// <summary>
	/// Called every loop of Update().
	/// 
	/// *** Make sure if this method is used that IsFinished is set to true at the end of it.
	/// *** If IsFinished is not set, the enemy controller will never move onto the next event in the list.
	/// </summary>
	/// <param name="gameManager">A reference to the game manager object.</param>
	/// <param name="enemyController">A reference to the enemy controller that called this event.</param>
	/// <param name="playerController">A reference to the player controller in the scene.</param>
	public abstract void Execute (GameManager gameManager, EnemyController enemyController, PlayerController playerController);
}

// Enemy Events List:

// Halt movement AI (DONE)
// Bullet Pattern (DONE)
// Pursue target (DONE)
// Orbit target
// Jump away from target
// Jump towards target
// Delay execution of events (DONE)