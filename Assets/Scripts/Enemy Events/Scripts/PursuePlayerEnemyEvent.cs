using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewPursuePlayerEnemyEvent", menuName = "Enemy Events/Pursue Player Enemy Event")]
public class PursuePlayerEnemyEvent : EnemyEvent {
	public override void Initialize (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		enemyController.MoveTowardsPosition(playerController.transform.position);

		IsFinished = true;
	}

	public override void Execute (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}