using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Editors:				Frank Alfano
// Date Created:		10/05/22
// Date Last Editted:	10/05/22

public class DebugManager : MonoBehaviour {
	[SerializeField] private Text debugHealthText;
	[SerializeField] private Text debugAmmoText;
	[SerializeField] private Text debugFPSText;
	[Space]
	[SerializeField] private PlayerController playerController;

	private float deltaTime;
	private float fps;

	/// <summary>
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	private void OnValidate ( ) {
		playerController = FindObjectOfType<PlayerController>( );
	}

	/// <summary>
	/// Called when this entity object is created
	/// </summary>
	private void Start ( ) {
		OnValidate( );
	}

	/// <summary>
	/// Called as fast as possible as the game is running.
	/// </summary>
	private void Update ( ) {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		fps = 1.0f / deltaTime;

		UpdateDisplay( );
	}

	/// <summary>
	/// Update the debug text on the screen with script variable values
	/// </summary>
	private void UpdateDisplay ( ) {
		debugFPSText.text = $"FPS: {fps}";
		debugHealthText.text = $"Player Health: {playerController.CurrentHealth}";
		debugAmmoText.text = $"Player Ammo: {playerController.CurrentAmmo}";
	}
}
