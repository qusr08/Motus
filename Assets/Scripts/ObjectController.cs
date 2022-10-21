using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/19/22
// Date Last Editted:	10/19/22

public class ObjectController : MonoBehaviour {
	[SerializeField] protected GameController gameController;
	[SerializeField] protected Rigidbody2D rigidBody2D;
	[SerializeField] protected SpriteRenderer spriteRenderer;
	[SerializeField] protected Animator animator;

	/// <summary>
	/// Update variables each time the Unity Editor is refreshed.
	/// </summary>
	protected void OnValidate ( ) {
		// Get references to these components/gameobjects automatically
		gameController = FindObjectOfType<GameController>( );
		rigidBody2D = GetComponent<Rigidbody2D>( );
		spriteRenderer = GetComponent<SpriteRenderer>( );
		animator = GetComponent<Animator>( );
	}

	/// <summary>
	/// Called when this object is created
	/// </summary>
	protected void Start ( ) {
		OnValidate( );
	}

	/// <summary>
	/// Called as fast as possible while the game is running
	/// </summary>
	protected void Update ( ) {
		// The further down something is, the higher the sorting order should be
		spriteRenderer.sortingOrder = (int) (transform.position.y * -100f);
	}
}
