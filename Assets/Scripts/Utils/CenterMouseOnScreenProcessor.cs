using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

// Editors: Frank Alfano
// Date Created: 9/14/22
// Date Last Editted: 9/15/22

// The purpose of this is to shift the position of the mouse input in the Input Actions window
// This way the mouse properly aims the player without doing logic inside of the PlayerController script

// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/Processors.html

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class CenterMouseOnScreenProcessor : InputProcessor<Vector2> {
#if UNITY_EDITOR
	static CenterMouseOnScreenProcessor ( ) {
		Initialize( );
	}
#endif

	// https://forum.unity.com/threads/possible-bug-with-custom-processor.779081/
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void Initialize ( ) {
		InputSystem.RegisterProcessor<CenterMouseOnScreenProcessor>( );
	}

	public override Vector2 Process (Vector2 value, InputControl control) {
		return value - new Vector2(Screen.width / 2, Screen.height / 2);
	}
}
