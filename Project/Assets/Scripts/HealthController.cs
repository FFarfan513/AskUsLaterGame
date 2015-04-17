using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

	private static int lives;

	// Use this for initialization
	void Start () {
		lives = 8;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("Lives: " + lives);
	}

	public static void DecrementLives(){
		lives--;
		if (lives == 0) {
			Debug.Log ("death");
			Application.LoadLevel(0);
		}
	}

	void OnGUI() {
		GUIStyle style = new GUIStyle ();
		style.fontSize = 20;
		style.normal.textColor = Color.white;
		GUI.Label(new Rect(279, 10, 100, 20), lives.ToString (), style);

	}
}
