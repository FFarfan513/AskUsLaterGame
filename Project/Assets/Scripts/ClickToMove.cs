﻿using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Camera cam;
	private float distanceFromCamera;
	public int mouseButton; //left click is 0, right click is 1
	private Vector3 mousePos;
	private Vector3 relativeMouse;
	private MoveTo player;
	private IdleController idler;

	//let's Idler know if we've done a valid click this Update.
	private bool validClick;

	private LayerMask enemies;

	void Start() {
		idler = cam.GetComponent<IdleController>();
		idler.IsIdling(false);
		player = cam.GetComponent<CenterOn>().character.GetComponent<MoveTo>();
		distanceFromCamera = CenterOn.cameraZ;
		enemies = LayerMask.GetMask("Enemy");
	}
	
	void Update () {
		validClick = false;
		if (Time.timeScale == 1) {
			//once the mouseButton is pressed, the object goes directly to that position
			if (Input.GetMouseButtonDown(mouseButton)) {

				if (WithinScreen()) {
					if (idler.GetIsIdle())
						idler.IsIdling(false);
					validClick = true;
					transform.position = cam.ScreenToWorldPoint(mousePos);
				}
			}
			//if the other mouse button was pressed, see if there is an enemy you can neutralize there
			else if (Input.GetMouseButtonDown((mouseButton+1)%2)) {

				if (WithinScreen()) {
					//Perform a raycast from your click position directly through the scene
					Ray r = cam.ScreenPointToRay(mousePos);
					RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(r,100,enemies);
					//If you've clicked on at least one enemy, freeze all of them.
					if (hits.Length > 0) {
						if (idler.GetIsIdle())
							idler.IsIdling(false);
						validClick = true;
						foreach(var hit in hits)
							hit.transform.gameObject.GetComponent<EnemyController>().Neutralize();
					}
				}
			}

			//If you haven't pressed a button, and you're stationary: start the idling.
			if (!validClick && !player.GetIsMoving()) {
				if (!idler.GetIsIdle())
					idler.IsIdling(true);
			}
		}
	}

	//checks if the click was within the borders of the camera
	bool WithinScreen() {
		//If this is the first click, get the idleController started
		if (!idler.Started())
			idler.Go();
		mousePos = Input.mousePosition;
		mousePos.z=distanceFromCamera;
		relativeMouse = cam.ScreenToViewportPoint(mousePos);
		return (relativeMouse.x<=1.0 && relativeMouse.x>=0);
	}

}
