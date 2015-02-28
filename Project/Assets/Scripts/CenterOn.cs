using UnityEngine;
using System.Collections;

public class CenterOn : MonoBehaviour {
	public GameObject character;
	public Camera cam;

	//This matches the camera's Z in unity
	public static float cameraZ = 10f;
	//The width of the divider wall between White and Black
	public static float dividerWallWidth = 10f;

	private float cameraXSpeed;
	private float cameraYSpeed;
	public float speed;
	private float edge;

	void Start() {
		cameraXSpeed = 0;
		cameraYSpeed = 0;
		edge = 0.58f; //edge keeps track of the relative percentage of the screen we're allowed to move in
	}

	void Update () {
		Vector3 cameraPosition = cam.transform.position;

		//the mins and maxes are the relative screen points at which the camera will start moving
		float xMax = (new Vector3(edge,0,cameraZ)).x;
		float xMin = (new Vector3((1-edge),0,cameraZ)).x;;
		float yMax = (new Vector3(0,edge,cameraZ)).y;
		float yMin = (new Vector3(0,(1-edge),cameraZ)).y;
		Vector3 relativePos = cam.WorldToViewportPoint(character.transform.position);

		if ( relativePos.x > xMax ) {
			cameraXSpeed = speed;
		}
		else if (relativePos.x < xMin) {
			cameraXSpeed = -speed;
		}
		else {
			cameraXSpeed = 0;
		}

		if ( relativePos.y > yMax ) {
			cameraYSpeed = speed;
		}
		else if (relativePos.y < yMin) {
			cameraYSpeed = -speed;
		}
		else {
			cameraYSpeed = 0;
		}

		cameraPosition.x += cameraXSpeed;
		cameraPosition.y += cameraYSpeed;
		transform.position = cameraPosition;
	}
}
