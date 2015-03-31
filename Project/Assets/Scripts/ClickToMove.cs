using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Camera cam;
	public float distanceFromCamera = CenterOn.cameraZ;
	public int mouseButton; //left click is 0, right click is 1
	private Vector3 currentPosition;

	private LayerMask enemies;

	void Start() {
		enemies = LayerMask.GetMask("Enemy");
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
		//if the other mouse button was pressed, see if there is an enemy you can neutralize there
		if (Input.GetMouseButtonDown((mouseButton+1)%2)) {
			Ray r = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(r,100,enemies);
			foreach(var hit in hits)
				hit.transform.gameObject.GetComponent<EnemyController>().Neutralize();
		}
	}

}
