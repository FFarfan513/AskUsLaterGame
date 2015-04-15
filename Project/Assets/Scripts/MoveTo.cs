using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {
	private Vector3 currentPosition;
	private Vector3 heading;
	private Vector3 moveHere;
	public bool isMoving;
	public GameObject followMe;
	public float moveSpeed;
	private float bounceDist = 0.15f;
	
	//public GameObject myRipplePrefab;
	//public float rippleSpeed;
	//private float timestamp;

	void Start () {
		currentPosition = transform.position;
		//timestamp = 0f;
	}
	
	void Update () {
		currentPosition = transform.position;
		moveHere = followMe.transform.position;
		heading = (moveHere - currentPosition).normalized;
		//if the distance between us and our target is less than .1, teleport there.
		//this is to prevent that weird shaking that happens a lot.
		if (Vector3.Distance(currentPosition,moveHere) < 0.1f) {
			transform.position = moveHere;
			isMoving = false;
		}
		else {
			Vector3 target = heading * moveSpeed + currentPosition;
			transform.position = Vector3.Lerp(currentPosition, target, Time.deltaTime);
			isMoving = true;
			/*
			if (Time.time >= timestamp) {
				Invoke("createRipple", 0.001F);
				timestamp = Time.time + rippleSpeed;
			}
			*/
		}
	}

	public Vector3 getHeading() {
		return heading;
	}

	void OnCollisionEnter2D(Collision2D other) {
		//this makes us bounce back a bit, so that we can't phase into the wall
		if (other.gameObject.tag == "Wall") {
			transform.position += (-heading*bounceDist);
			followMe.transform.position = transform.position;
		}
	}

	void OnCollisionStay2D() {
		//This is so that in case the game thinks we've already "entered" the wall, and it won't trigger the OnCollisionEnter method
		followMe.transform.position = transform.position;
	}

	/*
	void createRipple() {
		if (myRipplePrefab != null) {
			Instantiate(myRipplePrefab, transform.position, Quaternion.identity);
		}
	}
	*/
}
