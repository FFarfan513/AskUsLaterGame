using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Camera cam;
	public float distanceFromCamera = CenterOn.cameraZ;
	public int mouseButton; //left click is 0, right click is 1
	private Vector3 currentPosition;

	void Start() {
		currentPosition = transform.position;
	}
	
	void Update () {
			//once the mouseButton is pressed, the object goes directly to that position
			if (Input.GetMouseButtonDown(mouseButton)) {
			var mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			currentPosition = cam.ScreenToWorldPoint(mousePos);
			transform.position = currentPosition;
		}
	}

}
