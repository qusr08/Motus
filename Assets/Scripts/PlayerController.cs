using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Editors:				Frank Alfano
// Date Created:		09/12/22
// Date Last Editted:	10/28/22

public class PlayerController : EntityController {
	[Space]
	[SerializeField] private Transform aimObject;
	[SerializeField] public UIBarController HealthBarController;
	[SerializeField] public UIBarController DashBarController;
	[SerializeField] public UIBarController ChargedBulletBarController;
	[Space]
	[SerializeField] [Min(0f)] private float dashDistance;
	[SerializeField] [Min(0f)] private float dashTime;
	[SerializeField] [Min(0f)] private float dashCooldownTime;
	[SerializeField] [Min(0f)] private float dashRegenerationTime;
	[SerializeField] private ParticleSystem dashParticleSystem;
	[Space]
	[SerializeField] public bool IsDeflecting;
	[SerializeField] private GameObject shieldObject;
	[Space]
	[SerializeField] public bool IsShooting;
	[SerializeField] public float shootDelayTime;

	private float shootDelayTimer;

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
			return (DashBarController.Percentage >= 0.25f);
		}
	}

	private new void Start ( ) {
		base.Start( );

		HealthBarController.Percentage = 1f;
		DashBarController.Percentage = 1f;
		ChargedBulletBarController.Percentage = 0f;
	}

	/// <summary>
	/// Called as fast as possible while the game is running
	/// </summary>
	private new void Update ( ) {
		base.Update( );

		// If the player is no longer alive
		// ... destroy the player game object (FOR NOW)
		if (!IsAlive) {
			return;
		}

		// Make player shots rapid fire
		if (IsShooting) {
			if (shootDelayTimer <= 0f) {
				// Spawn a bullet in a certain direction
				gameController.SpawnBullet(transform.position, AimAngleDegrees, BulletType.PLAYER, 900);

				// Reset the shoot delay timer
				shootDelayTimer = shootDelayTime;

				// DEBUG STATS
				gameController.BulletsFired++;
			} else {
				shootDelayTimer -= Time.deltaTime;
			}
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
		} else if (IsDashOnCooldown) {
			dashCooldownTimer -= Time.deltaTime;
		} else {
			DashBarController.Percentage += dashRegenerationTime * Time.deltaTime;
		}

		// Move the gun aiming object
		aimObject.gameObject.SetActive(Aim.magnitude > 0);
		aimObject.localPosition = Aim * 0.5f;
		aimObject.rotation = Quaternion.Euler(0, 0, AimAngleDegrees + (Mathf.Abs(AimAngleDegrees) > 90f ? -135f : -45f));
		aimObject.GetComponent<SpriteRenderer>( ).flipX = (Mathf.Abs(AimAngleDegrees) > 90f);

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
		HealthBarController.Percentage = (float) CurrentHealth / MaxHealth;

		return returnValue;
	}

	public new float Heal (float heal)
	{
		float returnValue = base.Heal(heal);

		HealthBarController.Percentage = (float)CurrentHealth / MaxHealth;

		return returnValue;
	}

	/// <summary>
	/// Called whenever input is detected that will make the player move.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnMove (InputValue value) {
		// If the player is dashing, prevent them from aiming
		if (IsDashing) {
			return;
		}

		// If the game controller is in a gamestate that pauses the game
		// ... do not update player controls
		if (!gameController.IsPlaying) {
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

		// If the game controller is in a gamestate that pauses the game
		// ... do not update player controls
		if (!gameController.IsPlaying) {
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

		// If the game controller is in a gamestate that pauses the game
		// ... do not update player controls
		if (!gameController.IsPlaying) {
			return;
		}

		IsShooting = (value.Get<float>( ) == 1);

		// If the player is just now pressing the button to shoot, make a bullet immediately shoot
		if (IsShooting) {
			shootDelayTimer = 0f;
		}
	}

	/// <summary>
	/// Called whenever input is detected that will make the player shoot a charged bullet.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnSpecialShoot (InputValue value) {
		// If the player is dashing, prevent them from shooting
		// If the player is not aiming, then do not try to shoot in a certain direction
		// If the player is deflecting, prevent them from shooting
		if (IsDashing || !IsAiming || IsDeflecting) {
			return;
		}

		// If the game controller is in a gamestate that pauses the game
		// ... do not update player controls
		if (!gameController.IsPlaying) {
			return;
		}

		// If the player does not have enough charged up to shoot a charged bullet
		// ... dont shoot the bullet lol
		if (ChargedBulletBarController.Percentage < 0.25f) {
			return;
		}

		// Spawn a bullet in a certain direction
		gameController.SpawnBullet(transform.position, AimAngleDegrees, BulletType.CHARGED, 500);

		// Remove one full charged bullet bar
		ChargedBulletBarController.Percentage -= 0.25f;

		// DEBUG STATS
		gameController.ChargedBulletsFired++;
	}

	/// <summary>
	/// Called whenever input is detected that will make the player dash.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnDash (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		// If the player is not moving, then do not try to dash in a certain direction
		// If the player is deflecting, prevent them from breaking out of it with a dash
		if (IsDashing || !IsMoving || IsDeflecting || IsShooting) {
			return;
		}

		// If the game controller is in a gamestate that pauses the game
		// ... do not update player controls
		if (!gameController.IsPlaying) {
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
		RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Movement, dashDistance, (1 << LayerMask.NameToLayer("Arena")) | (1 << LayerMask.NameToLayer("Enemy")));

		// If the player hits something
		// ... make the dash distance meet the object the player hit
		if (hit2D) {
			// The dash distance is now the distance between the position of the player and the point that the RaycastHit hit
			// Also subtract 1f to place the player a little bit away from the object hit by the dash
			newDashDistance = Mathf.Clamp(Vector2.Distance(transform.position, hit2D.point) - 1f, 0, dashDistance);
		}

		// Set the positions that dictate the players dash
		fromDashPosition = transform.position;
		toDashPosition = transform.position + (Vector3) (Movement * newDashDistance);

		// Reset the dash time
		// This equation makes sure that even if the player hits something with the dash, the speed of the dash stays the same
		dashTimer = dashTime - (newDashDistance / dashDistance * dashTime);
		dashCooldownTimer = dashCooldownTime;

		// Remove one full dash bar
		DashBarController.Percentage -= 0.25f;

		// Set the dash particle system to the proper rotation
		dashParticleSystem.transform.rotation = Quaternion.Euler(0, 0, (Mathf.Rad2Deg * Mathf.Atan2(Movement.y, Movement.x)) + 90f);
		dashParticleSystem.Play( );

		// DEBUG STATS
		gameController.DashesUsed++;
	}

	/// <summary>
	/// Called whenever input is detected that will make the player deflect.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnDeflect (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		// If the player is not aiming, then do not try to deflect in a certain direction
		if (IsDashing || !IsAiming || IsShooting) {
			return;
		}

		// If the game controller is in a gamestate that pauses the game
		// ... do not update player controls
		if (!gameController.IsPlaying) {
			return;
		}

		IsDeflecting = (value.Get<float>( ) > 0);
		shieldObject.SetActive(IsDeflecting);
	}

	/// <summary>
	/// Called when the player pauses the game.
	/// </summary>
	/// <param name="value">The value of the control input.</param>
	public void OnPause (InputValue value) {
		gameController.GameState = (gameController.GameState == GameState.GAME ? GameState.PAUSE : GameState.GAME);
	}
}
