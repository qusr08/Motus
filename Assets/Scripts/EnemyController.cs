using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date Created: 9/16/22
// Date Last Editted: 9/16/22

// *** DUMMY CLASS This is definitely gonna change later just wanted to have some starting point

public class EnemyController : MonoBehaviour {
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform target;
	[SerializeField] private float shootTime;

	private float timer;

	private void Start ( ) {
		timer = shootTime;
	}

	private void Update ( ) {
		if (target != null) {
			// transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(transform.position.y - target.position.y, transform.position.x - target.position.y) + 90f);

			timer -= Time.deltaTime;
			if (timer <= 0) {
				BulletController.SpawnBullet(bulletPrefab, transform.position, (target.position - transform.position).normalized, BulletType.ENEMY);
				timer = shootTime;
			}
		}
	}

	// Have the player lose health
	// int damage: The amount of health to make the player lose
	public void TakeDamage (int damage) {
		// TO DO: Implement this later
	}
}

// Bullet pattern planning (PROLLY WILL CHANGE)

// BulletPattern
// List<ahiwdba>
//		{BulletType, Angle, Delay}
//		{BulletType, Angle, Delay}
//		{.....

// ShootPattern()
//		for all list items:
//			SpawnBullet(...)