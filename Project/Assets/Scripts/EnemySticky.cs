using UnityEngine;
using System.Collections;

public class EnemySticky : MonoBehaviour {

	EnemyController controller;
	private bool canMove;
	private Vector3 followPos;

	void Start() {
		controller = gameObject.GetComponent<EnemyController>();
		canMove = true;
	}

	void Update () {
		if (canMove && controller.GetState() != EnemyController.State.Paralyzed) {
			followPos = controller.followMe.transform.position;
			transform.position = controller.DumbSeek(transform.position, followPos);
		}
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if (other.tag == controller.GetPlayerTag()) {
			controller.Damage();
			controller.KillMe();
		}
		else if (other.tag == "Wall"){
			canMove = false;	// if collides into wall, 
		}
	}
}
