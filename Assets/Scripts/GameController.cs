using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/09/22
// Date Last Editted:	10/22/22

public enum GameState {
	GAME, PAUSE, GAMEOVER
}

public class GameController : MonoBehaviour {
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private PlayerController playerController;
	[Space]
	[SerializeField] private Transform GameUI;
	[SerializeField] private Transform GameOverUI;
	[Space]
	[SerializeField] private GameState gameState = GameState.GAME;
	[Space]
	[SerializeField] private Texture2D crosshairCursorTexture;
	[SerializeField] private Vector2 crosshairCursorHotspot;

	public GameState GameState {
		get {
			return gameState;
		}

		set {
			// Update the previous state and disable and UI that should not be shown anymore
			switch (gameState) {
				case GameState.GAME:
					GameUI.gameObject.SetActive(false);

					break;
				case GameState.GAMEOVER:
					GameOverUI.gameObject.SetActive(false);

					break;
			}

			// Set the gamestate value
			gameState = value;

			// Update the new state UI to be visible
			switch (gameState) {
				case GameState.GAME:
					GameUI.gameObject.SetActive(true);

					// Make time move at normal speed
					Time.timeScale = 1f;

					break;
				case GameState.GAMEOVER:
					GameOverUI.gameObject.SetActive(true);

					// Stop time
					Time.timeScale = 0f;

					break;
			}
		}
	}

	private void OnValidate ( ) {
		playerController = FindObjectOfType<PlayerController>( );
	}

	private void Start ( ) {
		OnValidate( );

		GameState = GameState.GAME;

		Cursor.SetCursor(crosshairCursorTexture, crosshairCursorHotspot, CursorMode.Auto);
	}

	private void Update ( ) {
		// Update different parts of the game depending on the gamestate
		switch (gameState) {
			case GameState.GAME:
				if (!playerController.IsAlive) {
					GameState = GameState.GAMEOVER;
				}

				break;
		}
	}

	/// <summary>
	/// Spawn a bullet that moves in a certain direction from a starting point.
	/// </summary>
	/// <param name="position">The position to start the bullet from.</param>
	/// <param name="bulletAngleDegrees">The angle in degrees to shoot the bullet in. Right is 0 degrees (like the unit circle).</param>
	/// <param name="bulletType">The type of bullet to shoot.</param>
	/// <param name="bulletSpeed">The speed of the bullet. Don't put anything to use the default speed in the BulletController class.</param>
	public void SpawnBullet (Vector2 position, float bulletAngleDegrees, BulletType bulletType, float bulletSpeed = -1) {
		// Spawn a bullet at the location of the player
		BulletController bullet = Instantiate(bulletPrefab, position, Quaternion.identity).GetComponent<BulletController>( );

		// Set the direction of the bullet to the direction the player is currently aiming
		bullet.Direction = new Vector2(Mathf.Cos(bulletAngleDegrees * Mathf.Deg2Rad), Mathf.Sin(bulletAngleDegrees * Mathf.Deg2Rad));
		bullet.BulletType = bulletType;
		// Use the default bullet speed unless a new one was put into the method
		if (bulletSpeed > 0) {
			bullet.BulletSpeed = bulletSpeed;
		}

		// Since all of the variables of the bullet have been set, it can be initialized
		// This flag means that the bullet can now function properly
		bullet.IsInitialized = true;
	}
}
