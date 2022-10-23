using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editors:				Frank Alfano
// Date Created:		10/22/22
// Date Last Editted:	10/22/22

public class MenuController : MonoBehaviour {
	private void Awake ( ) {
		// Make sure that the menu controller object does not get destroyed when a new scene is loaded
		DontDestroyOnLoad(gameObject);
	}

	public void LoadScene (string scene) {
		SceneManager.LoadScene(scene);
	}

	public void OpenURL (string url) {
		Application.OpenURL(url);
	}

	public void Quit ( ) {
		Application.Quit( );
	}
}
