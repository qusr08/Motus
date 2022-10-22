using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/22/22

[CreateAssetMenu(fileName = "NewJumpToDistanceFromPlayerEnemyEvent", menuName = "Enemy Events/Jump To Distance From Player Enemy Event")]
public class JumpToDistanceFromPlayerEnemyEvent : EnemyEvent {
	[Tooltip("The distance from the player to have the enemy jump to.")]
	[SerializeField] private float jumpToDistance;
	[Tooltip("How high the enemy should jump.")]
	[SerializeField] private float jumpHeight;
	[Tooltip("How long it should take the enemy to complete its jump.")]
	[SerializeField] private float jumpTime;

	private Vector2 fromJumpPosition;
	private Vector2 toJumpPosition;
	private float jumpTimer;

	/// <summary>
	/// The progress through the jump.
	/// </summary>
	private float JumpProgress {
		get {
			return (jumpTimer / jumpTime);
		}
	}

	public override void StartEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		enemyController.IsJumping = true;
		IsFinished = false;
		jumpTimer = 0f;

		// Get the ratio between the distance the enemy is at relative to the player and the distance it needs to go to
		float distanceRatio = jumpToDistance / enemyController.DistanceToPlayer;

		// Calculate the starting and ending positions for the jump
		fromJumpPosition = enemyController.transform.position;
		toJumpPosition = (enemyController.transform.position * distanceRatio) + (playerController.transform.position * (1 - distanceRatio));
	}

	public override void UpdateEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		// Get the current height based on the jump progress that the enemy should be at
		float height = -(4 * jumpHeight * JumpProgress) * (JumpProgress - 1);
		// Get the ground position of the enemy during its jump
		Vector2 groundPosition = Vector2.Lerp(fromJumpPosition, toJumpPosition, JumpProgress);

		// Set the position of the enemy directly (not seeking the position because the enemy is leaving the ground)
		enemyController.transform.position = groundPosition + new Vector2(0, height);
		enemyController.Shadow.position = groundPosition;

		// If the jump has been completed
		// ... exit out of the event
		// ... if not continue to increase the time that has elapsed during the jump
		if (JumpProgress == 1) {
			enemyController.IsJumping = false;
			IsFinished = true;
		} else {
			jumpTimer = Mathf.Clamp(jumpTimer + Time.deltaTime, 0, jumpTime);
		}
	}
}
