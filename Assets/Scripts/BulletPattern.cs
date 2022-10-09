using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/08/22
// Date Last Editted:	10/08/22

[CreateAssetMenu(fileName = "NewBulletPattern", menuName = "Bullet Pattern")]
public class BulletPattern : ScriptableObject {
	[Tooltip("A list of all bullets in this bullet pattern.")]
	[SerializeField] public List<BulletInstruction> bulletInstructions;
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