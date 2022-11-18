using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// Editors:				Frank Alfano
// Date Created:		10/09/22
// Date Last Editted:	11/18/22

public enum GameState {
	GAME, WAVE, PAUSE, GAMEOVER
}

public class GameController : MonoBehaviour {
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private PlayerController playerController;
	[Space]
	[SerializeField] private Transform gameUI;
	[SerializeField] private Transform gameOverUI;
	[SerializeField] private Transform waveUI;
	[SerializeField] private Transform pauseUI;
	[SerializeField] private TextMeshProUGUI waveDisplayText;
	[SerializeField] private TextMeshProUGUI gameStatsText;
	[Tooltip("How long the wave UI should stay on screen before enemies spawn again.")]
	[SerializeField] private float waveUITime;
	[Space]
	[SerializeField] private GameState gameState = GameState.GAME;
	[SerializeField] private bool _isPlaying;
	[SerializeField] private bool DoSpawningEnemyWaves = true;
	[Space]
	[SerializeField] public int WaveCount;
	[SerializeField] public int BulletsFired;
	[SerializeField] public int BulletsHit;
	[SerializeField] public int ChargedBulletsFired;
	[SerializeField] public int ChargedBulletsHit;
	[SerializeField] public int BulletsDashedThrough;
	[SerializeField] public int BulletsDeflected;
	[SerializeField] public int EnemiesDefeated;
	[SerializeField] public int DashesUsed;
	[Space]
	[SerializeField] private Texture2D crosshairCursorTexture;
	[SerializeField] private Vector2 crosshairCursorHotspot;
	[SerializeField] private Texture2D normalCursorTexture;
	[SerializeField] private Vector2 normalCursorHotspot;
	[Space]
	[SerializeField] private Transform enemyParentContainer;
	[SerializeField] private List<GameObject> enemyPrefabList;
	[Tooltip("The values of the costs should line up with the corresponding index in the enemyPrefabList array.")]
	[SerializeField] private List<int> enemyTypeCosts;

	private float waveUITimer = 0;
	// A list of INDECIES corresponding to the enemy prefabs to spawn
	private List<int> waveEnemies = new List<int>( );

	/// <summary>
	/// The current amount of enemies that are in the map
	/// </summary>
	public int EnemyCount {
		get {
			return enemyParentContainer.childCount;
		}
	}

	/// <summary>
	/// Whether or not the game is currently updating player/enemies.
	/// </summary>
	public bool IsPlaying {
		get {
			return _isPlaying;
		}

		set {
			_isPlaying = value;

			// Set the time scale
			Time.timeScale = (_isPlaying ? 1f : 0f);

			// Set the cursor
			if (_isPlaying) {
				Cursor.SetCursor(crosshairCursorTexture, crosshairCursorHotspot, CursorMode.Auto);
			} else {
				Cursor.SetCursor(normalCursorTexture, normalCursorHotspot, CursorMode.Auto);
			}
		}
	}

	/// <summary>
	/// The current state of the game
	/// </summary>
	public GameState GameState {
		get {
			return gameState;
		}

		set {
			// Update the previous state and disable and UI that should not be shown anymore
			switch (gameState) {
				case GameState.GAME:
					gameUI.gameObject.SetActive(false);

					break;
				case GameState.PAUSE:
					pauseUI.gameObject.SetActive(false);

					break;
				case GameState.WAVE:
					waveUI.gameObject.SetActive(false);

					foreach (int enemyTypeIndex in waveEnemies) {
						// Spawn in the enemy according to the array type
						Vector2 randomPosition = FindObjectOfType<CalculateArenaBounds>( ).GetRandomPositionInArena( );
						Transform enemy = Instantiate(enemyPrefabList[enemyTypeIndex], randomPosition, Quaternion.identity).transform;
						enemy.SetParent(enemyParentContainer, true);
					}

					break;
				case GameState.GAMEOVER:
					gameOverUI.gameObject.SetActive(false);

					break;
			}

			// Set the gamestate value
			gameState = value;

			// Update the new state UI to be visible
			switch (gameState) {
				case GameState.GAME:
					gameUI.gameObject.SetActive(true);
					IsPlaying = true;

					break;
				case GameState.PAUSE:
					pauseUI.gameObject.SetActive(true);
					IsPlaying = false;

					break;
				case GameState.WAVE:
					WaveCount++;

					// Equation to determine difficulty per wave: 4x + 1
					// float costLimit = (4 * WaveCount) + 1;
					// Equation to determine difficulty per wave: 0.3636x^2 + 4.6363
					float costLimit = (0.3636f * WaveCount * WaveCount) + 4.6363f;

					List<int> availableEnemyTypeIndecies = new List<int>( );
					waveEnemies.Clear( );

					// Get random enemies to spawn in the wave
					if (WaveCount % 10 == 0) {
						for (int i = 0; i < WaveCount / 10; i++) {
							waveEnemies.Add(4);
						}
					} else {
						while (costLimit > 0) {
							for (int i = 0; i < enemyTypeCosts.Count; i++) {
								// If the cost is higher than what is avaiable
								// ... do not allow that enemy type to spawn
								if (enemyTypeCosts[i] > costLimit) {
									continue;
								}

								// If the enemy type is able to spawn, add it to the array
								availableEnemyTypeIndecies.Add(i);
							}

							// If there are no enemies that can spawn with the remaining cost limit left
							// ... break out of the spawning loop
							if (availableEnemyTypeIndecies.Count == 0) {
								break;
							}

							// Add a random enemy index to the array of enemies that will spawn during the next wave
							int randomEnemyTypeIndex = availableEnemyTypeIndecies[Random.Range(0, availableEnemyTypeIndecies.Count)];
							waveEnemies.Add(randomEnemyTypeIndex);

							// Subtract the cost that it takes to spawn that enemy
							costLimit -= enemyTypeCosts[randomEnemyTypeIndex];

							availableEnemyTypeIndecies.Clear( );
						}
					}

					// Update the wave UI
					waveDisplayText.text = $"< wave #{WaveCount} >";
					waveUI.gameObject.SetActive(true);
					waveUITimer = waveUITime;
					IsPlaying = true;

					break;
				case GameState.GAMEOVER:
					gameStatsText.text =
						$"waves survived: {WaveCount - 1}\n" +
						$"enemies defeated: {EnemiesDefeated}\n" +
						$"bullets deflected: {BulletsDeflected}\n" +
						$"bullets dashed through: {BulletsDashedThrough}\n" +
						$"dashes used: {DashesUsed}\n" +
						$"dash accuracy: {((float) BulletsDashedThrough / DashesUsed) * 100}%\n" +
						$"bullets fired: {BulletsFired}\n" +
						$"bullets hit: {BulletsHit}\n" +
						$"shot accuracy: {((float) BulletsHit / BulletsFired) * 100}%\n" +
						$"charged bullets fired: {ChargedBulletsFired}\n" +
						$"charged bullets hit: {ChargedBulletsHit}\n" +
						$"charged shot accuracy: {((float) ChargedBulletsHit / ChargedBulletsFired) * 100}%";
					gameOverUI.gameObject.SetActive(true);
					IsPlaying = false;

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
	}

	private void Update ( ) {
		// Update different parts of the game depending on the gamestate
		switch (gameState) {
			case GameState.GAME:
				// If the player has died
				// ... go to the game over state
				if (!playerController.IsAlive) {
					GameState = GameState.GAMEOVER;
				}

				// If all enemies have been killed
				// ... spawn the next wave
				if (DoSpawningEnemyWaves && EnemyCount == 0) {
					GameState = GameState.WAVE;
				}

				break;
			case GameState.WAVE:
				// If the wave state has been active for a certain amount of time
				// ... automatically switch to the game state where enemies will spawn
				waveUITimer -= Time.deltaTime;
				if (waveUITimer <= 0f) {
					GameState = GameState.GAME;
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
