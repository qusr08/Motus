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
	[SerializeField] private float jumpTime;

	private Vector2 fromJumpPosition;
	private Vector2 jumpApexPosition;
	private Vector2 toJumpPosition;
	private float jumpTimer;
	private float a;

	private float JumpProgress {
		get {
			return (jumpTimer / jumpTime);
		}
	}

	public override void StartEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		enemyController.IsJumping = true;
		IsFinished = false;

		float distanceRatio = jumpToDistance / enemyController.DistanceToPlayer;

		fromJumpPosition = enemyController.transform.position;
		toJumpPosition = (enemyController.transform.position * distanceRatio) + (playerController.transform.position * (1 - distanceRatio));

		a = (4 * jumpHeight) / ((fromJumpPosition.x - toJumpPosition.x) * (toJumpPosition.x - fromJumpPosition.x));
		jumpTimer = 0f;
	}

	public override void UpdateEvent (GameController gameController, EnemyController enemyController, PlayerController playerController) {
		float height = -(4 * jumpHeight * JumpProgress) * (JumpProgress - 1);
		Vector2 groundPosition = Vector2.Lerp(fromJumpPosition, toJumpPosition, JumpProgress);

		enemyController.transform.position = groundPosition + new Vector2(0, height);
		enemyController.Shadow.position = groundPosition;

		if (JumpProgress == 1) {
			enemyController.IsJumping = false;
			IsFinished = true;
		} else {
			jumpTimer = Mathf.Clamp(jumpTimer + Time.deltaTime, 0, jumpTime);
		}
	}
}
