using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Editors:				Frank Alfano, Steven Feldman
// Date Created:		09/12/22
// Date Last Editted:	10/05/22

public class PlayerController : Entity {
	[Space]
	[SerializeField] private Transform aimObject;
	[Space]
	[SerializeField] private float dashDistance;
	[SerializeField] private float dashSpeed;
	[SerializeField] public int CurrentAmmo;

	/// <summary>
	/// Whether or not the player is currently dashing.
	/// </summary>
	private Vector2 fromDashPosition;
	private Vector2 toDashPosition;
	private float dashTime;
	public bool IsDashing {
		get {
			return (dashTime < dashSpeed);
		}
	}

	/// <summary>
	/// Whether or not the player is currently deflecting.
	/// </summary>
	public bool IsDeflecting;

	/// <summary>
	/// Called as fast as possible while the game is running
	/// </summary>
	private void Update ( ) {
		// If the player is no longer alive
		// ... destroy the player game object (FOR NOW)
		if (!IsAlive) {
			Destroy(gameObject);
			return;
		}

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
		aimObject.rotation = Quaternion.Euler(0, 0, AimAngleDegrees);
	}

	/// <summary>
	/// Called whenever input is detected that will make the player move.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnMove (InputValue value) {
		// If the player is dashing, prevent them from moving
		if (IsDashing) {
			return;
		}

		Movement = value.Get<Vector2>( );
	}

	/// <summary>
	/// Called whenever input is detected that will make the player aim.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnAim (InputValue value) {
		// If the player is dashing, prevent them from aiming
		if (IsDashing) {
			return;
		}

		// Aim towards the direction of the controller joystick
		// Calculate the position and rotation of the aim arrow
		// The position of the aim arrow is also the direction the player is aiming
		Aim = value.Get<Vector2>( ).normalized;
		AimAngleDegrees = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x) - 90f;
	}

	/// <summary>
	/// Called whenever input is detected that will make the player shoot.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnShoot (InputValue value) {
		// If the player is dashing, prevent them from shooting
		// If the player is not aiming, then do not try to shoot in a certain direction
		// If the player is deflecting, prevent them from shooting
		if (IsDashing || !IsAiming || IsDeflecting) {
			return;
		}

		// If the player has no more ammo left, do not let them shoot
		if (CurrentAmmo == 0) {
			return;
		}

		// Spawn a bullet in a certain direction
		BulletController.SpawnBullet(bulletPrefab, transform.position, Aim, BulletType.PLAYER);
		CurrentAmmo--;
	}

	/// <summary>
	/// Called whenever input is detected that will make the player dash.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnDash (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		// If the player is not moving, then do not try to dash in a certain direction
		// If the player is deflecting, prevent them from breaking out of it with a dash
		if (IsDashing || !IsMoving || IsDeflecting) {
			return;
		}

		// 'newDashDistance' is the actual distance that the player is going to dash as the player might hit something
		float newDashDistance = dashDistance;

		// Send out a raycast in the direction of the dash to see if the player is going to hit something during the dash
		// The list is ordered from closest objects to furthest objects
		RaycastHit2D[ ] hits2D = Physics2D.RaycastAll(transform.position, Movement, dashDistance);

		// If the player hits something
		// ... make the dash distance meet the object the player hit
		for (int i = 0; i < hits2D.Length; i++) {
			// Make sure the player's dash doesn't stop because of bullets
			if (hits2D[i].transform.GetComponent<BulletController>( ) != null) {
				continue;
			}

			// The dash distance is now the distance between the position of the player and the point that the RaycastHit hit
			// Also subtract 1f to place the player a little bit away from the object hit by the dash
			newDashDistance = Vector2.Distance(transform.position, hits2D[i].point) - 1f;

			// Don't need to check the other hits because the closest thing was already found
			break;
		}

		// Set the positions that dictate the players dash
		fromDashPosition = transform.position;
		toDashPosition = transform.position + (Vector3) (Movement * newDashDistance);

		// Reset the dash time
		// This equation makes sure that even if the player hits something with the dash, the speed of the dash stays the same
		dashTime = dashSpeed - ((newDashDistance / dashDistance) * dashSpeed);
	}

	/// <summary>
	/// Called whenever input is detected that will make the player deflect.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnDeflect (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		// If the player is not aiming, then do not try to deflect in a certain direction
		if (IsDashing || !IsAiming) {
			return;
		}

		// TO DO: Maybe have the player move really slow while deflecting?
		//			Does deflecting act more like a shield or one time action like shooting?

		IsDeflecting = (value.Get<float>( ) > 0);
	}
}
