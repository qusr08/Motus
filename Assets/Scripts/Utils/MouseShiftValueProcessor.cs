using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

// Editors: Frank Alfano
// Date Created: 9/14/22
// Date Last Editted: 9/14/22

// The purpose of this is to shift the position of the mouse input in the Input Actions window
// This way the mouse properly aims the player without doing logic inside of the PlayerController script

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class MouseShiftValueProcessor : InputProcessor<Vector2> {
#if UNITY_EDITOR
	static MouseShiftValueProcessor ( ) {
		Initialize( );
	}
#endif

	[RuntimeInitializeOnLoadMethod]
	static void Initialize ( ) {
		InputSystem.RegisterProcessor<MouseShiftValueProcessor>( );
	}

	public override Vector2 Process (Vector2 value, InputControl control) {
		return value - new Vector2(Screen.width / 2, Screen.height / 2);
	}
}
