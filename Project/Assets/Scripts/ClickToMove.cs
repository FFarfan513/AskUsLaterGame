using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour {
	public Camera cam;
	private float distanceFromCamera;
	public int mouseButton; //left click is 0, right click is 1
	private Vector3 mousePos;
	private Vector3 relativeMouse;
	private MoveTo player;
	private IdleController idler;
	//let's Idler know if we've done a valid click this Update.
	private bool validClick;

	private LayerMask enemies;

	void Start() {
		idler = cam.GetComponent<IdleController>();
		idler.IsIdling(false);
		player = cam.GetComponent<CenterOn>().character.GetComponent<MoveTo>();
		distanceFromCamera = CenterOn.cameraZ;
		enemies = LayerMask.GetMask("Enemy");
	}
	
	void Update () {
		validClick = false;

		//once the mouseButton is pressed, the object goes directly to that position
		if (Input.GetMouseButtonDown(mouseButton)) {
			if (!idler.Started())
				idler.Go();
			mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			relativeMouse = cam.ScreenToViewportPoint(mousePos);

			if (WithinScreen()) {
				idler.IsIdling(false);
				validClick = true;
				transform.position = cam.ScreenToWorldPoint(mousePos);
			}
		}
		//if the other mouse button was pressed, see if there is an enemy you can neutralize there
		if (Input.GetMouseButtonDown((mouseButton+1)%2)) {
			if (!idler.Started())
				idler.Go();
			mousePos = Input.mousePosition;
			mousePos.z=distanceFromCamera;
			relativeMouse = cam.ScreenToViewportPoint(mousePos);

			if (WithinScreen()) {
				Ray r = cam.ScreenPointToRay(mousePos);
				RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(r,100,enemies);
				if (hits.Length > 0) {
					idler.IsIdling(false);
					validClick = true;
					foreach(var hit in hits)
						hit.transform.gameObject.GetComponent<EnemyController>().Neutralize();
				}
			}
		}

		//If you haven't pressed a button, and you're stationary: start the idling.
		if (!validClick && !player.isMoving) {
			idler.IsIdling(true);
		}
	}

	bool WithinScreen() {
		return (relativeMouse.x<=1.0 && relativeMouse.x>=0);
	}

}
