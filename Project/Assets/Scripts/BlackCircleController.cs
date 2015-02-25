using UnityEngine;
using System.Collections;

public class BlackCircleController : MonoBehaviour {
	Vector3 currentPosition;
	Vector3 moveDirection;
	public float moveSpeed = 5.0f;

	void Start () {
		currentPosition = transform.position;
	}
	
	void Update () {
		currentPosition = transform.position;
		Vector3 moveHere = ClickToMoveBlack.moveToHereBlack;
		float distanceBetween = Vector3.Distance(currentPosition,moveHere);
		//Debug.Log (moveHere + "vs" + currentPosition);
		if (distanceBetween < 0.1f) {
			transform.position = moveHere;
		}
		else {
			Vector3 moveToward = moveHere;
			moveDirection = moveToward - currentPosition;
			moveDirection.z = 0;
			moveDirection.Normalize ();
			
			Vector3 target = moveDirection * moveSpeed + currentPosition;
			transform.position = Vector3.Lerp(currentPosition, target, Time.deltaTime);
		}
	}
}
