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

	private int randomIndex;

	public override void StartEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		// Choose a random bullet pattern and run it
		randomIndex = Random.Range(0, bulletPatterns.Count);
		bulletPatterns[randomIndex].StartEvent(gameManager, enemyController, playerController);

		IsFinished = false;
	}

	public override void UpdateEvent (GameManager gameManager, EnemyController enemyController, PlayerController playerController) {
		bulletPatterns[randomIndex].UpdateEvent(gameManager, enemyController, playerController);

		if (bulletPatterns[randomIndex].IsFinished) {
			IsFinished = true;
		}
	}
}
