using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/09/22
// Date Last Editted:	10/09/22

[CreateAssetMenu(fileName = "NewBulletPatternEnemyEvent", menuName = "Enemy Events/Bullet Pattern Enemy Event")]
public class BulletPatternEnemyEvent : EnemyEvent {
	[Tooltip("A list of all bullets in this bullet pattern.")]
	[SerializeField] private List<BulletInstruction> bulletInstructions;

	public override void Initialize (GameManager gameManager, EnemyController enemyController) {
		// For each bullet instruction of the bullet pattern
		// ... spawn a bullet with the specified values
		foreach (BulletInstruction bulletInstruction in bulletInstructions) {
			gameManager.SpawnBullet(enemyController.transform.position, enemyController.AimAngleDegrees + bulletInstruction.BulletAngleOffsetDegrees, bulletInstruction.BulletSpeed, bulletInstruction.BulletType);
		}

		IsFinished = true;
	}

	public override void Execute (GameManager gameManager, EnemyController enemyController) {
		// Since the bullet pattern should just be spawned in the Initialize() method, if it reaches this part of the code something has gone wrong
		throw new System.NotImplementedException( );
	}
}

[System.Serializable]
public class BulletInstruction {
	[Tooltip("The type of bullet to shoot.")]
	[SerializeField] public BulletType BulletType = BulletType.ENEMY;
	[Tooltip("The angle offset to shoot the bullet at.\n\n0 means to shoot the bullet in the direction that the enemy is facing.")]
	[SerializeField] [Range(-180f, 180f)] public float BulletAngleOffsetDegrees = 0f;
	[Tooltip("The speed of the bullet.")]
	[SerializeField] [Min(0f)] public float BulletSpeed = 500f;
}