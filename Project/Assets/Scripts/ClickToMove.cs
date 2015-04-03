using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Camera cam;
	private float distanceFromCamera;
	public int mouseButton; //left click is 0, right click is 1
	bool hardmode = false;

	private LayerMask enemies;

	void Start() {
		distanceFromCamera = CenterOn.cameraZ;
		enemies = LayerMask.GetMask("Enemy");
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.H))
			hardmode = !hardmode;
		
		//once the mouseButton is pressed, the object goes directly to that position
		if (Input.GetMouseButtonDown(mouseButton)) {
			var mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			Vector3 relativeMouse = cam.ScreenToViewportPoint(mousePos);
			//THIS IF STATEMENT RIGHT HERE IS IMPORTANT:
			if (hardmode || (mouseButton==0 && relativeMouse.x<=1.1) || (mouseButton==1 && relativeMouse.x>=-0.1))
				transform.position = cam.ScreenToWorldPoint(mousePos);
		}
		//if the other mouse button was pressed, see if there is an enemy you can neutralize there
		if (Input.GetMouseButtonDown((mouseButton+1)%2)) {
			Ray r = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(r,100,enemies);
			foreach(var hit in hits)
				hit.transform.gameObject.GetComponent<EnemyController>().Neutralize();
		}
	}

}
