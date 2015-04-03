using UnityEngine;
using System.Collections;

public class GoalController : MonoBehaviour {

	private GameObject myPlayer;
	public bool goalReached;
	public float turnSpeed;
	public GameObject wallToRemove;
	//public GameObject myRipplePrefab;
	//public float rippleSpeed;
	//private float timestamp;
	
	void Start() {
		goalReached = false;
		myPlayer = findPlayer();
	}

	void Update() {
		Rotate();
		/*
		if (goalReached && Time.time >= timestamp) {
			Invoke("createRipple", 0.001F);
			timestamp = Time.time + rippleSpeed;
		}
		*/
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if (other.tag == myPlayer.tag) {
			if (wallToRemove == null) {		// if goal
				goalReached = true;
				turnSpeed *= 4f;
			}
			else {							// if switch
				Destroy(wallToRemove);
				Destroy(gameObject);
			}
		}
	}

	void OnTriggerExit2D( Collider2D other ) {
		if (other.tag == myPlayer.tag) {
			goalReached = false;
			turnSpeed /= 4f;
		}
	}

	private void Rotate() {
		transform.Rotate(Vector3.forward * turnSpeed * 50 * Time.deltaTime);
	}

	private GameObject findPlayer() {
		if (transform.position.x < 0)
			return GameObject.FindGameObjectWithTag("PlayerBlack");
		else
			return GameObject.FindGameObjectWithTag("PlayerWhite");
	}
}
