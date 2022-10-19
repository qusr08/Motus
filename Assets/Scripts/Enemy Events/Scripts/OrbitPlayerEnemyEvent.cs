using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/19/22

[CreateAssetMenu(fileName = "NewOrbitPlayerEnemyEvent", menuName = "Enemy Events/Orbit Player Enemy Event")]
public class OrbitPlayerEnemyEvent : EnemyEvent {
	[SerializeField] [Min(0f)] private float maximumRange;
	[SerializeField] [Min(0f)] private float minimumRange;
	[Tooltip("The range at which the enemy should orbit at, as long as the enemy is close enough to the player.")]
	[SerializeField] [Min(0f)] private float orbitRange;

	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		if (enemyController.DistanceToPlayer >= minimumRange && enemyController.DistanceToPlayer < maximumRange) {
			// Convert play movement to an angular velocity
			float angularVelocity = enemyController.MoveSpeed / orbitRange * Time.deltaTime;

			// Calculate the angle to move the enemy to
			// This is just the angle the enemy is already around the player plus the velocity
			// Have the direction the enemy travels depend on its instance id
			float moveToAngle = enemyController.AngleRadiansAroundPlayer + (angularVelocity * (enemyController.ModValue % 2 == 0 ? 1 : -1));

			// Calculate the position to move the enemy to based on the angle
			Vector2 moveToPosition = (Vector2) playerController.transform.position + (orbitRange * new Vector2(Mathf.Cos(moveToAngle), Mathf.Sin(moveToAngle)));
			enemyController.SeekPosition(moveToPosition);
		}

		IsFinished = true;
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}
