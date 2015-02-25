using UnityEngine;
using System.Collections;

public class ClickToMoveBlack : MonoBehaviour {
	public Camera cam;
	public float distanceFromCamera = CenterOn.cameraZ;
	public static Vector3 moveToHereBlack;

	void Start() {
		moveToHereBlack = transform.position;
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			var mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			moveToHereBlack = cam.ScreenToWorldPoint(mousePos);
			float theDivider = 0 - CenterOn.dividerWallWidth/2;
			if (moveToHereBlack.x < theDivider) {
				transform.position = moveToHereBlack;
			}
			else {
				moveToHereBlack.x = theDivider;
				transform.position = moveToHereBlack;
			}
		}
	}
}
