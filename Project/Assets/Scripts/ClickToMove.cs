using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Camera cam;
	private float distanceFromCamera;
	public int mouseButton; //left click is 0, right click is 1
	private Vector3 mousePos;
	private Vector3 relativeMouse;

	private LayerMask enemies;

	void Start() {
		distanceFromCamera = CenterOn.cameraZ;
		enemies = LayerMask.GetMask("Enemy");
	}
	
	void Update () {
		
		//once the mouseButton is pressed, the object goes directly to that position
		if (Input.GetMouseButtonDown(mouseButton)) {
			mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			relativeMouse = cam.ScreenToViewportPoint(mousePos);

			if (WithinScreen())
				transform.position = cam.ScreenToWorldPoint(mousePos);
		}
		//if the other mouse button was pressed, see if there is an enemy you can neutralize there
		if (Input.GetMouseButtonDown((mouseButton+1)%2)) {
			mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			relativeMouse = cam.ScreenToViewportPoint(mousePos);

			if (WithinScreen()) {
				Ray r = cam.ScreenPointToRay(mousePos);
				RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(r,100,enemies);
				foreach(var hit in hits)
					hit.transform.gameObject.GetComponent<EnemyController>().Neutralize();
			}
		}
	}

	bool WithinScreen() {
		if (relativeMouse.x<=1.0 && relativeMouse.x>=0)
			return true;
		else
			return false;
	}

}
