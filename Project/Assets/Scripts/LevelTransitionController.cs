using UnityEngine;
using System.Collections;

public class LevelTransitionController : MonoBehaviour {

	private GameObject[] goals;
	private readonly int maxHP = 8;
	private static int HP, levelCounter;
	private bool g1, g2;

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	void Start() {
		HP = maxHP;
		levelCounter = 0;
		goals = GameObject.FindGameObjectsWithTag("Goal");
	}

	void Update() {
		if (goals != null && goals[0] != null && goals[1] != null) {
			
			g1 = goals[0].GetComponent<GoalController>().goalReached;
			g2 = goals[1].GetComponent<GoalController>().goalReached;
			
			if ( g1 && g2 ) {
				goals = null;
				levelCounter++;
				loadNextLevel();
			}
		}
	}

	void loadNextLevel() {
		HP = maxHP;
		Application.LoadLevel("Level"+levelCounter);
		goals = GameObject.FindGameObjectsWithTag("Goal");
	}

	public static void DecrementHP(){
		HP--;
		if (HP <= 0) { // Gameover -> Continue Scene
			Debug.Log ("death");
			Application.LoadLevel("Continue");
		}
	}
	
	void OnGUI() {
		if (HP > 0) {
			GUIStyle style = new GUIStyle ();
			style.fontSize = (int)(Screen.height*0.05);
			style.normal.textColor = Color.white;
			GUI.Label( new Rect( (float)(Screen.width*0.495), (float)(Screen.height*0.025), 100, 20 ), HP.ToString(), style );
		}
		else {
			GUIStyle style = new GUIStyle ();
			style.fontSize = (int)(Screen.height*0.1);
			style.normal.textColor = Color.white;
			GUI.Label( new Rect( (float)(Screen.width*0.5), (float)(Screen.height*0.025), 100, 20 ), "Gameover", style );
		}
	}

}
