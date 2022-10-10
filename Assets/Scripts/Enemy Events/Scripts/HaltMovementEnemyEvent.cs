using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewHaltMovementEnemyEvent", menuName = "Enemy Events/Halt Movement Enemy Event")]
public class HaltMovementEnemyEvent : EnemyEvent {
	public override void Initialize (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		enemyController.MoveTowardsPosition(enemyController.transform.position);

		IsFinished = true;
	}

	public override void Execute (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		// Since the bullet pattern should just be spawned in the Initialize() method, if it reaches this part of the code something has gone wrong
		throw new System.NotImplementedException( );
	}
}
