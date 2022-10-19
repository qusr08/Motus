using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Editors:				Frank Alfano, Steven Feldman
// Date Created:		09/12/22
// Date Last Editted:	10/19/22

public class PlayerController : EntityController {
	[Space]
	[SerializeField] private Transform aimObject;
	[SerializeField] private UIBarController healthBarController;
	[SerializeField] private UIBarController dashBarController;
	[Space]
	[SerializeField] [Min(0f)] private float dashDistance;
	[SerializeField] [Min(0f)] private float dashTime;
	[SerializeField] [Min(0f)] private float dashCooldownTime;
	[SerializeField] [Min(0f)] private float dashRegenerationTime;
	[SerializeField] [Min(0f)] public int CurrentAmmo;
	[SerializeField] [Min(0f)] public float BulletSpeed;

	private Vector2 fromDashPosition;
	private Vector2 toDashPosition;
	private float dashTimer;
	private float dashCooldownTimer;
	/// <summary>
	/// Whether or not the player is currently dashing.
	/// </summary>
	public bool IsDashing {
		get {
			return (dashTimer < dashTime);
		}
	}
	public bool IsDashOnCooldown {
		get {
			return (dashCooldownTimer > 0);
		}
	}
	public bool CanDash {
		get {
			return (dashBarController.Percentage >= 0.25f);
		}
	}

	/// <summary>
	/// Whether or not the player is currently deflecting.
	/// </summary>
	public bool IsDeflecting;

	private new void Start ( ) {
		base.Start( );

		healthBarController.Percentage = 1f;
		dashBarController.Percentage = 1f;
	}

	/// <summary>
	/// Called as fast as possible while the game is running
	/// </summary>
	private new void Update ( ) {
		base.Update( );

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
			transform.position = Vector2.Lerp(fromDashPosition, toDashPosition, dashTimer / dashTime);

			// Increment the 'dashTime' by the time that has passed
			dashTimer += Time.deltaTime;

			// Debug.Log("Dashed!");
		} else if (IsDashOnCooldown) {
			dashCooldownTimer -= Time.deltaTime;
			// Debug.Log("Cooldown!");
		} else {
			dashBarController.Percentage += dashRegenerationTime * Time.deltaTime;
			// Debug.Log("Increment!");
		}

		// Move the aiming object
		aimObject.localPosition = Aim;
		aimObject.rotation = Quaternion.Euler(0, 0, AimAngleDegrees - 45f);
		// Make sure to flip the gun based on the aim angle around the player
		// This way the gun sprite is always facing up
		// aimObject.GetComponent<SpriteRenderer>( ).flipX = (Mathf.Abs(AimAngleDegrees) > 90f);

		// Update the animator
		animator.SetFloat("MovementX", Movement.x);
		animator.SetFloat("MovementY", Movement.y);
	}

	/// <summary>
	/// Override the Damage() method in the EntityController class for more functionality.
	/// </summary>
	/// <param name="damage">The damage to deal to the player.</param>
	/// <returns>The current health of the player after.</returns>
	public new float Damage (float damage) {
		float returnValue = base.Damage(damage);

		// Update the health bar
		healthBarController.Percentage = (float) CurrentHealth / MaxHealth;

		return returnValue;
	}

	/// <summary>
	/// Called whenever input is detected that will make the player move.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnMove (InputValue value) {
		// If the player is dashing, prevent them from moving
		/*if (IsDashing) {
			return;
		}*/

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
		AimAngleDegrees = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x);
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
		gameManager.SpawnBullet(transform.position, AimAngleDegrees, BulletSpeed, BulletType.PLAYER);
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

		// If the dash is still on cooldown
		// ... prevent the player from dashing
		if (IsDashOnCooldown) {
			return;
		}

		// If the player cannot dash, as in one dash bar has not been fully regenerated
		// ... return from the method
		if (!CanDash) {
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
			if (hits2D[i].transform.GetComponent<BulletController>( ) != null || hits2D[i].transform.GetComponent<PlayerController>( ) != null) {
				continue;
			}

			// The dash distance is now the distance between the position of the player and the point that the RaycastHit hit
			// Also subtract 1f to place the player a little bit away from the object hit by the dash
			newDashDistance = Mathf.Clamp(Vector2.Distance(transform.position, hits2D[i].point) - 1f, 0, dashDistance);

			// Don't need to check the other hits because the closest thing was already found
			break;
		}

		// Set the positions that dictate the players dash
		fromDashPosition = transform.position;
		toDashPosition = transform.position + (Vector3) (Movement * newDashDistance);

		// Reset the dash time
		// This equation makes sure that even if the player hits something with the dash, the speed of the dash stays the same
		dashTimer = dashTime - (newDashDistance / dashDistance * dashTime);
		dashCooldownTimer = dashCooldownTime;

		// Remove one full dash bar
		dashBarController.Percentage -= 0.25f;
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
