using UnityEngine;
using System.Collections;

public class LevelTransitionController : MonoBehaviour {

	private GameObject[] goals;
	private readonly int maxHP = 8;
	private static int HP;
	public static int thisLevel;
	private bool g1, g2;
	public GameObject health;
	private static GameObject healthZero;
	private static GameObject[] healthBarUp;
	private static GameObject[] healthBarDown;
	private float yscale;

	void Start() {
		yscale = health.transform.localScale.y;
		HP = maxHP;
		healthZero = null;
		healthBarUp = new GameObject[maxHP+1];
		healthBarDown = new GameObject[maxHP+1];
		DrawHP();
		thisLevel = Application.loadedLevel;;
		goals = GameObject.FindGameObjectsWithTag("Goal");
	}

	void DrawHP() {
		healthZero = Instantiate(health, new Vector3(0,0,0), Quaternion.identity) as GameObject;
		healthZero.name = "HP1";
		for (int i=2; i<=maxHP; i++) {
			healthBarUp[i] = Instantiate(health, new Vector3(0,(i-1)*(yscale*2),0), Quaternion.identity) as GameObject;
			healthBarDown[i] = Instantiate(health, new Vector3(0,-(i-1)*(yscale*2),0), Quaternion.identity) as GameObject;
			healthBarUp[i].name = "HP"+i;
			healthBarDown[i].name = "HP"+i;
		}
	}

	void Update() {
		if (goals != null && goals[0] != null && goals[1] != null) {
			
			g1 = goals[0].GetComponent<GoalController>().goalReached;
			g2 = goals[1].GetComponent<GoalController>().goalReached;
			
			if ( g1 && g2 ) {
				goals = null;
				loadNextLevel();
			}
		}
	}

	void loadNextLevel() {
		HP = maxHP;
		Application.LoadLevel((thisLevel+1));
		GenerateGraph.graphLoaded = false;
		goals = GameObject.FindGameObjectsWithTag("Goal");
	}

	public static void DecrementHP(){
		if (HP == 1) {
			GameObject.Destroy(healthZero);
		}
		else {
			GameObject.Destroy(healthBarUp[HP]);
			GameObject.Destroy(healthBarDown[HP]);
		}
		HP--;
		if (HP <= 0) { // Gameover -> Continue Scene
			Application.LoadLevel("Continue");
		}
	}

}
