using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewJumpToDistanceFromPlayerEnemyEvent", menuName = "Enemy Events/Jump To Distance From Player Enemy Event")]
public class JumpToDistanceFromPlayerEnemyEvent : EnemyEvent {
	[Tooltip("The distance from the player to have the enemy jump to.")]
	[SerializeField] private float distanceFromPlayer;
	
	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}
