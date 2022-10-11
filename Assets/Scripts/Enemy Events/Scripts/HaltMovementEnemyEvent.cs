using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewHaltMovementEnemyEvent", menuName = "Enemy Events/Halt Movement Enemy Event")]
public class HaltMovementEnemyEvent : EnemyEvent {
	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		enemyController.MoveTowardsPosition(enemyController.transform.position);

		IsFinished = true;
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}
