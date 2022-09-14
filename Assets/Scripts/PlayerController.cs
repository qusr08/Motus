using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Editors: Frank Alfano
// Date Created: 9/12/22
// Date Last Editted: 9/14/22

public class PlayerController : MonoBehaviour {
	[SerializeField] private Transform aimObject;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private new Rigidbody2D rigidbody2D;
	[Space]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float dashDistance;
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

		// Move the aiming object
		aimObject.localPosition = Aim;
		aimObject.rotation = Quaternion.Euler(0, 0, aimAngle);
	}

	private void FixedUpdate ( ) {
		// Move the player
		rigidbody2D.velocity = moveSpeed * Time.deltaTime * Movement;
	}

	// Have the player lose health
	// int damage: The amount of health to make the player lose
	public void TakeDamage (int damage) {
		// TO DO: Implement this later
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
		aimAngle = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x) - 90f;
	}

	public void OnShoot (InputValue value) {
		// If the player is dashing, prevent them from shooting
		// If the player is not aiming, then do not try to shoot in a certain direction
		if (IsDashing || !IsAiming) {
			return;
		}

		BulletController.SpawnBullet(bulletPrefab, transform.position, Aim, BulletType.PLAYER);
	}

	public void OnDash (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		// If the player is not aiming, then do not try to dash in a certain direction
		if (IsDashing || !IsAiming) {
			return;
		}

		// 'newDashDistance' is the actual distance that the player is going to dash as the player might hit something
		float newDashDistance = dashDistance;

		// Send out a raycast in the direction of the dash to see if the player is going to hit something during the dash
		RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Aim, dashDistance);
		// If the player hits something
		// ... make the dash distance meet the object the player hit
		if (hit2D) {
			// The dash distance is now the distance between the position of the player and the point that the RaycastHit hit
			// Also subtract 0.75f to place the player a little bit away from the object hit by the dash
			newDashDistance = Vector2.Distance(transform.position, hit2D.point) - 0.75f;
		}

		// Set the positions that dictate the players dash
		fromDashPosition = transform.position;
		toDashPosition = transform.position + (Vector3) (Aim * newDashDistance);

		// Reset the dash time
		// This equation makes sure that even if the player hits something with the dash, the speed of the dash stays the same
		dashTime = dashSpeed - ((newDashDistance / dashDistance) * dashSpeed);

		// Reset movement so the player doesn't continue to move after exiting a dash
		Movement = Vector2.zero;
	}

	public void OnDeflect (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		// If the player is not aiming, then do not try to deflect in a certain direction
		if (IsDashing || !IsAiming) {
			return;
		}

		Debug.Log("Deflect");
	}
}
