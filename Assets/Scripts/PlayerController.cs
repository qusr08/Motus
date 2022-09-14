using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Editors: Frank Alfano
// Date: 9/12/22

public class PlayerController : MonoBehaviour {
	[SerializeField] private Transform aimObject;
	[SerializeField] private GameObject bulletPrefab;
	[Space]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float dashLength;
	[SerializeField] private float dashSpeed;
	[Space]
	[SerializeField] public Vector2 Aim;
	[SerializeField] public Vector2 Movement;

	private float aimAngle;
	private Vector2 fromDashPosition;
	private Vector2 toDashPosition;
	private float dashTime;

	public bool IsDashing {
		get {
			return (dashTime < dashSpeed);
		}
	}

	public bool IsAiming {
		get {
			return (Aim.magnitude > 0);
		}
	}

	private void Update ( ) {
		// If the player is dashing
		// ... update the position based on the dash
		if (IsDashing) {
			// Lerp between the last position of the player and the new dash position
			// 'dashTime' is the time the player has taken as it travels between the two points
			// Dividing 'dashTime' by 'dashSpeed' will get a value between 0 and 1 which is used to linearly interpolate between the two points
			transform.position = Vector2.Lerp(fromDashPosition, toDashPosition, dashTime / dashSpeed);
			// Increment the 'dashTime' by the time that has passed
			dashTime += Time.deltaTime;
		}

		// Move the aiming direction
		aimObject.localPosition = Aim;
		aimObject.rotation = Quaternion.Euler(0, 0, aimAngle);

		// Move the player
		transform.position += (Vector3) (moveSpeed * Time.deltaTime * Movement);
	}

	public void OnMove (InputValue value) {
		// If the player is dashing, prevent them from moving
		if (IsDashing) {
			return;
		}

		Movement = value.Get<Vector2>( );
	}

	public void OnAim (InputValue value) {
		// If the player is dashing, prevent them from aiming
		if (IsDashing) {
			return;
		}

		// Get the new aim value from the controller
		Vector2 newAim = value.Get<Vector2>( );

		// If the joystick is centered (not moving)
		// ... return and don't set a new aim value.
		// This is so the aim arrow always stays visible next to the player
		/*if (newAim.magnitude == 0) {
			return;
		}*/

		// Calculate the position and rotation of the aim arrow
		// The position of the aim arrow is also the direction the player is aiming
		Aim = newAim.normalized;
		aimAngle = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x) + 90;
	}

	public void OnShoot (InputValue value) {
		// If the player is dashing, prevent them from shooting
		if (IsDashing && !IsAiming) {
			return;
		}

		BulletController.SpawnBullet(bulletPrefab, transform.position, Aim, BulletType.PLAYER);
	}

	public void OnDash (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		if (IsDashing && !IsAiming) {
			return;
		}

		// Set the positions that dictate the players dash
		fromDashPosition = transform.position;
		toDashPosition = transform.position + (Vector3) (Aim * dashLength);
		// Reset the dash time
		dashTime = 0;

		// Reset movement so the player doesn't continue to move after exiting a dash
		Movement = Vector2.zero;
	}

	public void OnDeflect (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		if (IsDashing && !IsAiming) {
			return;
		}

		Debug.Log("Deflect");
	}
}
