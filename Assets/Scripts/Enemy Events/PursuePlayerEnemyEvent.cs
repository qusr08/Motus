using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewPursuePlayerEnemyEvent", menuName = "Enemy Events/Pursue Player Enemy Event")]
public class PursuePlayerEnemyEvent : EnemyEvent {
	[SerializeField] [Min(0f)] private float maximumRange;
	[SerializeField] [Min(0f)] private float minimumRange;

	public override void StartEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		if (enemyController.DistanceToPlayer >= minimumRange && enemyController.DistanceToPlayer < maximumRange) {
			enemyController.SeekPosition(playerController.transform.position);
		}

		IsFinished = true;
	}

	public override void UpdateEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}
