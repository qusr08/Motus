using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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
