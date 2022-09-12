using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date: 9/12/22

public class BulletController : MonoBehaviour {
	// *** 'lifetime' will probably be replaced with collision down the line, right now it is used to
	// despawn the bullets after a certain amount of time
	[SerializeField] private float lifetime;
	[SerializeField] private float bulletSpeed;
	
	public Vector2 Direction;

	private void Update ( ) {
		// If the direction of the bullet has not been set, then do not update the position or decrease the lifetime
		// Each time a bullet is instantiated this direction needs to be set
		if (Direction.magnitude == 0) {
			return;
		}
		
		// Move the position of the bullet
		transform.position += (Vector3) (Direction * bulletSpeed * Time.deltaTime);
		
		// Decrease the lifetime by how many seconds has passed
		lifetime -= Time.deltaTime;
		// If the lifetime has reached 0
		// ... destroy the bullet
		if (lifetime <= 0) {
			Destroy(gameObject);
		}
	}
}
