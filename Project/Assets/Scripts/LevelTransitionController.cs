﻿using UnityEngine;
using System.Collections;

public class LevelTransitionController : MonoBehaviour {

	private GameObject[] goals;
	private readonly int maxHP = 8;
	private static int HP, levelCounter;

	void Start() {
		HP = maxHP;
		levelCounter = 0;
		goals = GameObject.FindGameObjectsWithTag("Goal");
	}

	void Update() {		
		if (goals[0].GetComponent<GoalController>().goalReached &&
		    goals[1].GetComponent<GoalController>().goalReached) {
			levelCounter++;
			loadNextLevel();
		}
	}

	void loadNextLevel() {
		HP = maxHP;
		Application.LoadLevel("Level"+levelCounter);
		goals = GameObject.FindGameObjectsWithTag("Goal");
	}

	public static void DecrementHP(){
		HP--;
		if (HP <= 0) {
			Debug.Log ("death");
			PlayerPrefs.SetInt("level", levelCounter);
			Application.LoadLevel("Continue");
		}
	}
	
	void OnGUI() {
		GUIStyle style = new GUIStyle ();
		style.fontSize = (int)(Screen.height*0.05);
		style.normal.textColor = Color.white;
		GUI.Label( new Rect( (float)(Screen.width*0.495), (float)(Screen.height*0.025), 100, 20 ), HP.ToString(), style );
	}

}
