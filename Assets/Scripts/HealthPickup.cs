using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Editors:              Matthew Meyrowitz, Frank Alfano
//Date created:         11/7/2022
//Date Last Editted:    11/18/2022

public class HealthPickup : ObjectController {
	//detect if health pickup collided with player
	private void OnTriggerEnter2D (Collider2D collision) {
		GameObject collisionGameObject = collision.gameObject;

		//check what the health pickup collided with
		PlayerController playerCollision = collisionGameObject.GetComponent<PlayerController>( );

		//check for collision
		if (playerCollision != null) {
			playerCollision.Heal(3);
			//destroy health pickup
			Destroy(gameObject);
		}
	}
}
