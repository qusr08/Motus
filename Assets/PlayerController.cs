using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	[SerializeField] private Transform aimObject;
	[Space]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float dashLength;

	private Vector2 movement;
	private Vector2 aim;
	private float aimAngle;

	private void Start ( ) {
		aim = Vector2.up;
	}

	private void Update ( ) {
		// Move the aiming direction
		aimObject.localPosition = aim;
		aimObject.rotation = Quaternion.Euler(0, 0, aimAngle);

		// Move the player
		transform.position += (Vector3) (movement * moveSpeed * Time.deltaTime);
	}

	public void OnMove (InputValue value) {
		movement = value.Get<Vector2>( );
	}

	public void OnAim (InputValue value) {
		// Get the new aim value from the controller
		Vector2 newAim = value.Get<Vector2>( );

		// If the joystick is centered (not moving)
		// ... return and don't set a new aim value.
		// This is so the aim arrow always stays visible next to the player
		if (newAim.magnitude == 0) {
			return;
		}

		aim = value.Get<Vector2>( ).normalized;
		aimAngle = Mathf.Rad2Deg * Mathf.Atan2(aim.y, aim.x) + 90;
	}

	public void OnShoot (InputValue value) {
		Debug.Log("Shoot");
	}

	public void OnDash (InputValue value) {
		Debug.Log("Dash");
	}

	public void OnDeflect (InputValue value) {
		Debug.Log("Deflect");
	}
}
