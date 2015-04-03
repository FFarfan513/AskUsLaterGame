using UnityEngine;
using System.Collections;

public class CenterOn : MonoBehaviour {
	public GameObject character;
	private Camera cam;
	MoveTo ch;
	//This matches the camera's Z in unity
	public static float cameraZ;
	
	private float speed;
	private float edge = 0.56f;

	void Awake() {
		cam = this.camera;
		ch = character.GetComponent<MoveTo>();
		speed = ch.moveSpeed;
		cameraZ = -cam.transform.position.z;
	}

	void Update () {
		//the mins and maxes are the relative screen points at which the camera will start moving
		float xMax = (new Vector3(edge,0,cameraZ)).x;
		float xMin = (new Vector3((1-edge),0,cameraZ)).x;;
		float yMax = (new Vector3(0,edge,cameraZ)).y;
		float yMin = (new Vector3(0,(1-edge),cameraZ)).y;
		Vector3 relativePos = cam.WorldToViewportPoint(character.transform.position);
		Vector3 moveDirection = ch.getHeading();

		//if your position relative to the screen is past a certain point, start moving the camera
		Vector3 horiz = new Vector3(moveDirection.x,0,0);
		Vector3 vert = new Vector3(0,moveDirection.y,0);
		//The screen will translate in the same direction as the heading, at the same speed.
		if ( relativePos.x > xMax || relativePos.x < xMin) {
			transform.Translate(horiz*speed*Time.deltaTime);
		}

		if ( relativePos.y > yMax || relativePos.y < yMin) {
			transform.Translate(vert*speed*Time.deltaTime);
		}
	}
}
