using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewJumpAwayFromPlayerEnemyEvent", menuName = "Enemy Events/Jump Away From Player Enemy Event")]
public class JumpAwayFromPlayerEnemyEvent : EnemyEvent {
	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}

	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}
