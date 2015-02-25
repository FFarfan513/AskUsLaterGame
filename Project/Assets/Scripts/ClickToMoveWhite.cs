using UnityEngine;
using System.Collections;

public class ClickToMoveWhite : MonoBehaviour {
	public Camera cam;
	public float distanceFromCamera = CenterOn.cameraZ;
	public static Vector3 moveToHereWhite;
	
	void Start() {
		moveToHereWhite = transform.position;
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(1)) {
			var mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			moveToHereWhite = cam.ScreenToWorldPoint(mousePos);
			float theDivider = 0 + CenterOn.dividerWallWidth/2;
			if (moveToHereWhite.x > theDivider) {
				transform.position = moveToHereWhite;
			}
			else {
				moveToHereWhite.x = theDivider;
				transform.position = moveToHereWhite;
			}
		}
	}
}
