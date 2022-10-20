using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/20/22

[CreateAssetMenu(fileName = "NewHaltMovementEnemyEvent", menuName = "Enemy Events/Halt Movement Enemy Event")]
public class HaltMovementEnemyEvent : EnemyEvent {
	[SerializeField] [Min(0f)] private float delayTime;
	[Tooltip("How far the delay time can be randomly offset from a static value.")]
	[SerializeField] [Min(0f)] private float delayTimeError;

	private float startTime;
	private float delayTimer;

	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		IsFinished = false;
		enemyController.IsMovementHalted = true;

		startTime = Time.time;
		delayTimer = delayTime + Random.Range(-delayTimeError, delayTimeError);
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		// Wait a specified amount of time before finishing the event
		if (Time.time - startTime >= delayTimer) {
			IsFinished = true;
			enemyController.IsMovementHalted = false;
		}
	}
}
