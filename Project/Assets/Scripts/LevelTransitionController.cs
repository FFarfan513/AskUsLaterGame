using UnityEngine;
using System.Collections;

public class LevelTransitionController : MonoBehaviour {

	public GameObject goalLeft;
	public GameObject goalRight;
	private GoalController g1, g2;
	public static int thisLevel;

	public GameObject health;
	private readonly int maxHP = 8;
	private static int HP;
	private static GameObject healthZero;
	private static GameObject[] healthBarUp;
	private static GameObject[] healthBarDown;

	private float yscale;
	private bool giveUp;
	private bool fading;
	
	void Awake() {
		//Set up the goals
		if (Application.loadedLevelName != "Continue")
			thisLevel = Application.loadedLevel;
		if (goalLeft != null)
			g1 = goalLeft.GetComponent<GoalController>();
		if (goalRight != null)
			g2 = goalRight.GetComponent<GoalController>();

		//Set up the HealthBar
		if (health != null) {
			yscale = health.transform.localScale.y;
			HP = maxHP;
			healthZero = null;
			healthBarUp = new GameObject[maxHP+1];
			healthBarDown = new GameObject[maxHP+1];
			DrawHP();
		}
	}

	void DrawHP() {
		healthZero = Instantiate(health, new Vector3(0,0,0), Quaternion.identity) as GameObject;
		healthZero.name = "HP";
		//For each health point greater than one, create two blocks of HP, one above and the other below.
		for (int i=2; i<=maxHP; i++) {
			healthBarUp[i] = Instantiate(health, new Vector3(0,(i-1)*(yscale*2),0), Quaternion.identity) as GameObject;
			healthBarDown[i] = Instantiate(health, new Vector3(0,-(i-1)*(yscale*2),0), Quaternion.identity) as GameObject;
			healthBarUp[i].name = "HP"+i;
			healthBarDown[i].name = "HP"+i;
			//making each tick of HP a child of healthZero makes things cleaner in the Hierarchy.
			healthBarUp[i].transform.parent = healthZero.transform;
			healthBarDown[i].transform.parent = healthZero.transform;
		}
	}

	//If both goals have been reached, reset them and start fading to the next level
	void Update() {
		if (g1!=null && g2 !=null && g1.Reached() && g2.Reached() ) {
			g1.Reset();
			g2.Reset();
			StartCoroutine(Fading(true));
		}
		if (Input.GetKeyDown("escape") && Time.timeScale == 1f && !giveUp && thisLevel != 0) {
			giveUp = true;
			StartCoroutine(Fading(false));
		}
	}
	
	//Slows down time for a bit and fades the volume down.
	IEnumerator Fading(bool alive) {
		fading = true;
		Time.timeScale = 0.2f;
		for (int i=0; i<20; i++) {
			AudioListener.volume = Mathf.Round((AudioListener.volume-0.05f) * 100f) / 100f;
			yield return new WaitForSeconds(0.02f);
		}
		Time.timeScale = 1f;
		if (alive) {
			Application.LoadLevel((thisLevel+1));
		}
		else {
			Application.LoadLevel("Continue");
		}
	}

	void OnLevelWasLoaded(int level) {
		if (AudioListener.volume != 1f)
			AudioListener.volume = 1f;
	}

	public void DecrementHP() {
		if (!fading) {
			if (HP == 1) {
				GameObject.Destroy(healthZero);
			}
			else if (HP > 1) {
				GameObject.Destroy(healthBarUp[HP]);
				GameObject.Destroy(healthBarDown[HP]);
			}
			HP--;
			if (HP == 0) { // Gameover -> Continue Scene
				HP = -1;
				StartCoroutine(Fading(false));
			}
		}
	}

	public void Next() {
		g1.SetTrue();
		g2.SetTrue();
	}

}
