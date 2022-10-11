using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/09/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewDelayEnemyEvent", menuName = "Enemy Events/Delay Enemy Event")]
public class DelayEnemyEvent : EnemyEvent {
	[SerializeField] [Min(0f)] private float minimumDelayTime;
	[SerializeField] [Min(0f)] private float maximumDelayTime;

	private float startTime;
	private float delayTime;

	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		IsFinished = false;

		startTime = Time.time;
		delayTime = Random.Range(minimumDelayTime, maximumDelayTime);
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		// Wait a specified amount of time before finishing the event
		if (Time.time - startTime >= delayTime) {
			IsFinished = true;
		}
	}
}
