using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Editors: Frank Alfano
// Date: 9/3/22

public class PlayerInputTests : MonoBehaviour {
	// Print statements to console based on input from gamepads
	// You can also look at the Input Debugger to see inputs of any connected gamepads

	public void OnNorthButton (InputValue value) {
		Debug.Log("North Button Pressed");
	}

	public void OnEastButton (InputValue value) {
		Debug.Log("East Button Pressed");
	}

	public void OnSouthButton (InputValue value) {
		Debug.Log("South Button Pressed");
	}

	public void OnWestButton (InputValue value) {
		Debug.Log("West Button Pressed");
	}

	public void OnDPad (InputValue value) {
		Debug.Log($"D-Pad Button Pressed: {value.Get<Vector2>( )}");
	}

	public void OnLeftJoystick (InputValue value) {
		Debug.Log($"Left Joystick Moved: {value.Get<Vector2>( )}");
	}

	public void OnRightJoystick (InputValue value) {
		Debug.Log($"Right Joystick Moved: {value.Get<Vector2>( )}");
	}

	public void OnLeftTrigger (InputValue value) {
		Debug.Log("Left Trigger Pressed");
	}

	public void OnRightTrigger (InputValue value) {
		Debug.Log("Right Trigger Pressed");
	}

	public void OnLeftBumper (InputValue value) {
		Debug.Log("Left Bumper Pressed");
	}

	public void OnRightBumper (InputValue value) {
		Debug.Log("Right Bumper Pressed");
	}

	public void OnLeftJoystickPress (InputValue value) {
		Debug.Log("Left Joystick Pressed");
	}

	public void OnRightJoystickPress (InputValue value) {
		Debug.Log("Right Joystick Pressed");
	}

	public void OnSelect (InputValue value) {
		Debug.Log("Select Button Pressed");
	}

	public void OnStart (InputValue value) {
		Debug.Log("Start Button Pressed");
	}
}
