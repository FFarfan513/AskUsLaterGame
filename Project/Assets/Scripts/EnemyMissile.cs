using UnityEngine;
using System.Collections;

public class EnemyMissile : MonoBehaviour {
	EnemyController controller;
	private float initialSpeed;
	private Transform followMe;
	public float accel;

	void Start () {
		controller = gameObject.GetComponent<EnemyController>();
		initialSpeed = controller.moveSpeed;
		followMe = controller.followMe.transform;
	}

	void Update() {
		//The reason I switched these statements is because going into Idle is actually important.
		//enemies go into Idle after being unfrozen, and I wanted to reset the acceleration when it unfroze.
		switch (controller.GetState())
		{
		case EnemyController.State.HasSight:
			//HasSight is less about having actual sight, and more about being locked into a direction.
			MissileTowards();
			break;
		case EnemyController.State.Idle:
			controller.moveSpeed = initialSpeed;
			break;
		case EnemyController.State.Paralyzed:
			break;
		default:
			print("ERROR! The state was somehow set to something other than it's enum values.\n");
			break;
		}

		if (controller.GetState() != EnemyController.State.Paralyzed &&
		    controller.GetState() != EnemyController.State.HasSight) {
			if (controller.CanSeeIt(transform.position+(transform.up*initialSpeed))) {
				FaceMe();
			}
			controller.SetState(EnemyController.State.HasSight);
		}
	}

	void FaceMe() {
		Vector3 dir = followMe.transform.position - transform.position;
		transform.rotation = Quaternion.LookRotation(Vector3.forward,dir);
	}

	void MissileTowards() {
		controller.moveSpeed += accel;
		Vector3 movin = transform.up*controller.moveSpeed*Time.deltaTime;
		transform.position += movin;
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if (other.tag == "Wall") {
			//bounces you backwards
			transform.Translate((-transform.up)*controller.moveSpeed*Time.deltaTime);
			//resets the speed, flips the enemy backwards, and forces you to recheck your sight.
			transform.Rotate (new Vector3(0,0,180));
			controller.SetState (EnemyController.State.Idle);
		}
		if (other.tag == controller.GetPlayerTag()) {
			controller.Damage();
			controller.KillMe();
		}
	}
}
