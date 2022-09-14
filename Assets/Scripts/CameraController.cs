using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors: Frank Alfano
// Date: 9/12/22

public class CameraController : MonoBehaviour {
	[SerializeField] private PlayerController playerController;
	[Space]
	// How smooth the camera moves towards the target object
	[SerializeField] [Range(0f, 1f)] private float smoothness;
	[SerializeField] [Min(0f)] private float aimDistance;

	private Vector2 velocity;

	private void Start ( ) {
		// If the target is set to an object
		// ... Set the position instantly when the game starts so the camera starts locked on the target
		if (playerController != null) {
			SetXYPosition(playerController.transform.position);
		}
	}

	private void Update ( ) {
		// If the target is set to an object
		// ... Smoothly move towards the position of the target
		if (playerController != null) {
			SetXYPosition(Vector2.SmoothDamp(transform.position, playerController.transform.position + (Vector3) (playerController.Aim * aimDistance), ref velocity, smoothness));
		}
	}

	// Set the position of the camera object without changing the Z value
	// The camera needs to stay some distance away from the target in order for it to be in view of the camera
	// Vector2 position: The position to set
	private void SetXYPosition (Vector2 position) {
		transform.position = new Vector3(position.x, position.y, transform.position.z);
	}
}
