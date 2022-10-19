using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Editors:				Frank Alfano
// Date Created:		10/19/22
// Date Last Editted:	10/19/22

public class UIBarController : MonoBehaviour {
	[SerializeField] private RectTransform barColorObject;

	private Vector2 uiBarDimensions = new Vector2(524, 48);

	private float _percentage;
	public float Percentage {
		get {
			return _percentage;
		}

		set {
			_percentage = Mathf.Clamp(value, 0f, 1f);

			// Set the size of the UI bar
			barColorObject.sizeDelta = new Vector2(_percentage * uiBarDimensions.x, uiBarDimensions.y);
		}
	}
}
