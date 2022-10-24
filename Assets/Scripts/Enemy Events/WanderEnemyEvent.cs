using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/21/22
// Date Last Editted:	10/23/22

[CreateAssetMenu(fileName = "NewWanderEnemyEvent", menuName = "Enemy Events/Wander Enemy Event")]
public class WanderEnemyEvent : EnemyEvent {
	[SerializeField] [Min(0f)] private float maximumRange;
	[SerializeField] [Min(0f)] private float minimumRange;

	private Vector2 wanderPosition = Vector2.zero;

	public override void StartEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		if (enemyController.DistanceToPlayer >= minimumRange && enemyController.DistanceToPlayer < maximumRange) {
			// If the enemy gets close to their wander position
			// ... generate a new position to move to
			if (wanderPosition.magnitude == 0 || ((Vector2) enemyController.transform.position - wanderPosition).magnitude < 0.5f) {
				wanderPosition = FindObjectOfType<CalculateArenaBounds>( ).GetRandomPositionInArena( );
			}

			enemyController.SeekPosition(wanderPosition);
		}

		IsFinished = true;
	}

	public override void UpdateEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}
