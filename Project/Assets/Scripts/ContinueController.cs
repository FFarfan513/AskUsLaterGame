using UnityEngine;
using System.Collections;

public class ContinueController : MonoBehaviour {
	private static bool continuing;
	
	void Start() {
		continuing = false;
	}

	void OnMouseDown() {
		if (!continuing) {
			if (this.name == "Continue") {
				Application.LoadLevel (LevelTransitionController.thisLevel);
			}
			if (this.name == "MainMenu") {
				Application.LoadLevel (0);
			}
		}
	}
}
