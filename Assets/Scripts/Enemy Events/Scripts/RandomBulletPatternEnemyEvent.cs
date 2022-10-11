using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/10/22
// Date Last Editted:	10/10/22

[CreateAssetMenu(fileName = "NewRandomBulletPatternEnemyEvent", menuName = "Enemy Events/Random Bullet Pattern Enemy Event")]
public class RandomBulletPatternEnemyEvent : EnemyEvent {
	[Tooltip("Choose a random bullet pattern from this list each time the event is called.")]
	[SerializeField] private List<BulletPatternEnemyEvent> bulletPatterns;

	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		// Choose a random bullet pattern and run it
		int randomIndex = Random.Range(0, bulletPatterns.Count);
		bulletPatterns[randomIndex].StartEvent(gameManager, enemyController, playerController);

		IsFinished = true;
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		throw new System.NotImplementedException( );
	}
}
