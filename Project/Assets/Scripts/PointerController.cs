using UnityEngine;
using System.Collections;

public class PointerController : MonoBehaviour {

	public GameObject goal;
	private MoveTo player;
	private SpriteRenderer arrow;
	private readonly int waitTime = 60;
	private int countDown;

	void Start() {
		player = transform.parent.GetComponent<MoveTo>();
		arrow = transform.GetComponent<SpriteRenderer>();
	}

	void FixedUpdate() {
		//countsDown like a few other scripts have been doing.
		//If the player has not been moving for a certain amount of time, show the arrow.
		if (!player.GetIsMoving()) {
			countDown--;
			if (countDown<=0) {
				countDown = waitTime;
				arrow.enabled = true;
			}
		}
		else {
			if (countDown != waitTime)
				countDown = waitTime;
			arrow.enabled = false;
		}
		if((Time.timeScale>0 && Time.timeScale<1) && arrow.enabled)
			arrow.enabled = false;
		FaceTheGoal();
	}

	//This makes the arrow constantly rotate towards the goal object
	void FaceTheGoal() {
		Vector3 dir = goal.transform.position - transform.position;
		Quaternion look = Quaternion.LookRotation(Vector3.forward,dir);
		if (transform.rotation != look) {
			transform.rotation = look;
		}
	}
	
}
