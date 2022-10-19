using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/09/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewBulletPatternEnemyEvent", menuName = "Enemy Events/Bullet Pattern Enemy Event")]
public class BulletPatternEnemyEvent : EnemyEvent {
	[SerializeField] [Min(0f)] private float delayTime;
	[Tooltip("How far the delay time can be randomly offset from a static value.")]
	[SerializeField] [Min(0f)] private float delayTimeError;
	[Space]
	[Tooltip("A list of all bullets in this bullet pattern.")]
	[SerializeField] private List<BulletInstruction> bulletInstructions;

	private float startTime;
	private float delayTimer;

	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		IsFinished = false;

		startTime = Time.time;
		delayTimer = delayTime + Random.Range(-delayTimeError, delayTimeError);
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		// Wait a specified amount of time before finishing the event
		if (Time.time - startTime >= delayTimer) {
			// For each bullet instruction of the bullet pattern
			// ... spawn a bullet with the specified values
			foreach (BulletInstruction bulletInstruction in bulletInstructions) {
				gameManager.SpawnBullet(enemyController.transform.position, enemyController.AimAngleDegrees + bulletInstruction.BulletAngleOffsetDegrees, bulletInstruction.BulletType);
			}

			IsFinished = true;
		}
	}
}

[System.Serializable]
public class BulletInstruction {
	[Tooltip("The type of bullet to shoot.")]
	[SerializeField] public BulletType BulletType = BulletType.ENEMY;
	[Tooltip("The angle offset to shoot the bullet at.\n\n0 means to shoot the bullet in the direction that the enemy is facing.")]
	[SerializeField] [Range(-180f, 180f)] public float BulletAngleOffsetDegrees = 0f;
}