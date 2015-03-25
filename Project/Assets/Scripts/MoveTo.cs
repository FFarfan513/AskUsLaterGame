using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {
	Vector3 currentPosition;
	Vector3 moveDirection;
	public GameObject followMe;
	//public GameObject myRipplePrefab;
	public float moveSpeed;
	//public float rippleSpeed;
	//private float timestamp;

	void Start () {
		currentPosition = transform.position;
		//timestamp = 0f;
	}
	
	void Update () {
		currentPosition = transform.position;
		Vector3 moveHere = followMe.transform.position;
		float distanceBetween = Vector3.Distance(currentPosition,moveHere);
		//if the distance between us and our target is less than .1, teleport there.
		//this is to prevent that weird shaking that happens a lot.
		if (distanceBetween < 0.1f) {
			transform.position = moveHere;
		}
		else {
			Vector3 moveToward = moveHere;
			moveDirection = moveToward - currentPosition;
			moveDirection.z = 0;
			moveDirection.Normalize();
			
			Vector3 target = moveDirection * moveSpeed + currentPosition;
			transform.position = Vector3.Lerp(currentPosition, target, Time.deltaTime);
			/*
			if (Time.time >= timestamp) {
				Invoke("createRipple", 0.001F);
				timestamp = Time.time + rippleSpeed;
			}
			*/
		}
	}

	void OnTriggerEnter2D( Collider2D other ) {
		//this makes us bounce back a bit, so that we can't phase into the wall
		//specifically, we bounce back at .1 speed
		Vector3 trajectory = followMe.transform.position - transform.position;
		trajectory.x = -trajectory.x;
		trajectory.y = -trajectory.y;
		trajectory = Vector3.ClampMagnitude(trajectory,0.1f);
		currentPosition += trajectory;
		transform.position = currentPosition;
		//this moves the target's position (the X) to where we are now, so that we don't keep moving after we bounce.
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
