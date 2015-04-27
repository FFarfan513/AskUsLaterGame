using UnityEngine;
using System.Collections;

public class ContinueController : MonoBehaviour {
	private static bool continuing;
	private bool continueButton = false, mainMenuButton = false;
	
	void Start() {
		if (this.name == "Continue") {
			continueButton = true;
		}
		else if (this.name == "MainMenu") {
			mainMenuButton = true;
		}
		continuing = false;
	}
	
	void Update() {
		if (Input.GetKey("escape")) {
			Application.Quit();
		}
	}

	void OnMouseDown() {
		//if the Continue button is pressed, reload thisLevel.
		//if the MainMenu button is pressed, load the title (scene 0).
		if (!continuing) {
			if (continueButton) {
				Application.LoadLevel(LevelTransitionController.thisLevel);
			}
			if (mainMenuButton) {
				Application.LoadLevel(0);
			}
		}
	}
}
