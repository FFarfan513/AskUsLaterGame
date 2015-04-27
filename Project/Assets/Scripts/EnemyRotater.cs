using UnityEngine;
using System.Collections;

public class EnemyRotater : MonoBehaviour {

	EnemyController controller;

	public float aggressiveRotateSpeed, calmRotateSpeed;

	void Start () {
		controller = gameObject.GetComponent<EnemyController>();
	}
	
	void Update () {
		//If this enemy is not paralyzed, check for sight.
		if (controller.GetState() != EnemyController.State.Paralyzed) {
			if (controller.CanSeeIt(transform.position))
				controller.SetState(EnemyController.State.HasSight);
			else
				controller.SetState(EnemyController.State.Idle);
		}

		switch (controller.GetState())
		{
		case EnemyController.State.HasSight:
			//If this enemy has sight, it rotates faster and seeks towards the player
			Rotate(aggressiveRotateSpeed);
			transform.position = controller.DumbSeek(transform.position, controller.followMe.transform.position);
			break;
		case EnemyController.State.Idle:
			//If it doesn't have sight, it rotates slowly in place
			Rotate(calmRotateSpeed);
			break;
		case EnemyController.State.NoSight:
			break;
		case EnemyController.State.Paralyzed:
			break;
		default:
			print("ERROR! The state was somehow set to something other than it's enum values.\n");
			break;
		}
	}

	void Rotate(float turnSpeed) {
		transform.Rotate(Vector3.forward * turnSpeed * 50 * Time.deltaTime);
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if (other.tag == controller.GetPlayerTag()) {
			controller.Damage();
			controller.KillMe();
		}
	}
}
