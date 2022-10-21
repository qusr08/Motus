using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/21/22
// Date Last Editted:	10/21/22

public class FadeForegroundObject : MonoBehaviour {
	private float FOREGROUND_OBJECT_FADE_ALPHA = 0.25f;
	private float FOREGROUND_OBJECT_FADE_TIME = 0.25f;

	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private PlayerController playerController;

	private void OnValidate ( ) {
		spriteRenderer = GetComponent<SpriteRenderer>( );
		playerController = FindObjectOfType<PlayerController>( );
	}

	/// <summary>
	/// Called when another object enters a collision with this object
	/// </summary>
	/// <param name="collision">The object that was collided with.</param>
	private void OnTriggerEnter2D (Collider2D collision) {
		if (collision.gameObject == playerController.gameObject) {
			StopAllCoroutines( );
			StartCoroutine(FadeToAlpha(FOREGROUND_OBJECT_FADE_ALPHA));
		}
	}

	/// <summary>
	/// Called when another object exits a collision with this object
	/// </summary>
	/// <param name="collision">The other object in the collision.</param>
	private void OnTriggerExit2D (Collider2D collision) {
		if (collision.gameObject == playerController.gameObject) {
			StopAllCoroutines( );
			StartCoroutine(FadeToAlpha(1f));
		}
	}

	/// <summary>
	/// Fade the transparency of the sprite to a specified alpha.
	/// </summary>
	/// <param name="toAlpha">The alpha to fade the color to.</param>
	/// <returns></returns>
	private IEnumerator FadeToAlpha (float toAlpha) {
		Color color = spriteRenderer.color;
		float fromAlpha = color.a;
		float elapsedTime = 0f;
		
		while (elapsedTime < FOREGROUND_OBJECT_FADE_TIME) {
			elapsedTime += Time.deltaTime;

			// Set the color of the sprite renderer to the alpha as it linearly interpolates between the old value and the new one
			color.a = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / FOREGROUND_OBJECT_FADE_TIME);
			spriteRenderer.color = color;

			yield return null;
		}

		// Make sure to set the alpha to the exact value input into the function
		// During the while loop, if this code isn't here, the alpha might not be set to the exact value
		color.a = toAlpha;
		spriteRenderer.color = color;
	}
}
