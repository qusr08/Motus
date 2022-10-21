using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/09/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewDelayEnemyEvent", menuName = "Enemy Events/Delay Enemy Event")]
public class DelayEnemyEvent : EnemyEvent {
	[SerializeField] [Min(0f)] private float delayTime;
	[Tooltip("How far the delay time can be randomly offset from a static value.")]
	[SerializeField] [Min(0f)] private float delayTimeError;

	private float startTime;
	private float delayTimer;

	public override void StartEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		IsFinished = false;

		startTime = Time.time;
		delayTimer = delayTime + Random.Range(-delayTimeError, delayTimeError);
	}

	public override void UpdateEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		// Wait a specified amount of time before finishing the event
		if (Time.time - startTime >= delayTimer) {
			IsFinished = true;
		}
	}
}
