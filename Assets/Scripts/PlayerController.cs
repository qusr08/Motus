using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Editors: Frank Alfano; Steven Feldman
// Date Created: 9/12/22
// Date Last Editted: 10/03/22

public class PlayerController : MonoBehaviour {
	[SerializeField] private Text healthText;
	[SerializeField] private Text ammoText;
	[Space]
	[SerializeField] private Transform aimObject;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private new Rigidbody2D rigidbody2D;
	[Space]
	[SerializeField] private float moveSpeed;
	[SerializeField] private float dashDistance;
	[SerializeField] private float dashSpeed;
	[SerializeField] private int health;
	[SerializeField] private int ammo;
	[Space]
	[SerializeField] public Vector2 Aim;
	[SerializeField] public Vector2 Movement;
	[SerializeField] public bool IsDeflecting;

	private float aimAngle;

	private Vector2 fromDashPosition;
	private Vector2 toDashPosition;
	private float dashTime;

	public int AmmoCount
	{
		get
		{
			return ammo;
		}
		set
		{
			ammo = value;
		}
	}
	public bool IsDashing {
		get {
			return (dashTime < dashSpeed);
		}
	}

	public bool IsAlive {
		get {
			return (health > 0);
		}
	}

	public bool IsAiming {
		get {
			return (Aim.magnitude > 0);
		}
	}

	public bool IsMoving {
		get {
			return (Movement.magnitude > 0);
		}
	}

	private void Start ( ) {
		UpdateDisplay(healthText, "Player Health: " + health);

        UpdateDisplay(ammoText, "Ammo: " + ammo);
    }

	private void Update ( ) {
		if (!IsAlive) {
			CallAfterDelay.Create(3.0f, () =>
			{
				if (health <= 0)
				{
					Destroy(gameObject);
				}
			});
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
		aimObject.rotation = Quaternion.Euler(0, 0, aimAngle);

        UpdateDisplay(ammoText, "Ammo: " + ammo);
    }

	private void FixedUpdate ( ) {
		// Move the player
		rigidbody2D.velocity = moveSpeed * Time.fixedDeltaTime * Movement;
	}

	// Have the player lose health
	// int damage: The amount of health to make the player lose
	public void TakeDamage (int damage) {
		// TO DO: Implement this later
		health -= damage;

		if (health < 0)
		{
			health = 0;
		}

		UpdateDisplay(healthText, "Player Health: " + health);
	}

	public void SecondWind()
	{
		if (health <= 0)
		{
			health = 1;
		}

        UpdateDisplay(healthText, "Player Health: " + health);

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

		// Aim towards the direction of the controller joystick
		// Calculate the position and rotation of the aim arrow
		// The position of the aim arrow is also the direction the player is aiming
		Aim = value.Get<Vector2>( ).normalized;
		aimAngle = Mathf.Rad2Deg * Mathf.Atan2(Aim.y, Aim.x) - 90f;
	}

	public void OnShoot (InputValue value) {
		// If the player is dashing, prevent them from shooting
		// If the player is not aiming, then do not try to shoot in a certain direction
		// If the player is deflecting, prevent them from shooting
        if (IsDashing || !IsAiming || IsDeflecting) {
			return;
		}

		if (ammo == 0)
		{
			return;
		}
		else
		{
			ammo--;
		}

        UpdateDisplay(ammoText, "Ammo: " + ammo);

        BulletController.SpawnBullet(bulletPrefab, transform.position, Aim, BulletType.PLAYER);
	}

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

	public void OnDeflect (InputValue value) {
		// If the player is dashing, prevent them from dashing again as they are dashing
		// If the player is not aiming, then do not try to deflect in a certain direction
		if (IsDashing || !IsAiming) {
			return;
		}

		// TO DO: Maybe have the player move really slow while deflecting?
		//			Does deflecting act more like a shield or one time action like shooting?
		
		IsDeflecting = (value.Get<float>( ) > 0);

		Debug.Log(IsDeflecting ? "...Player is deflecting..." : "...Player is no longer deflecting...");
	}

	private void UpdateDisplay (Text displayText, string text) {
		displayText.text = text;
	}
}
