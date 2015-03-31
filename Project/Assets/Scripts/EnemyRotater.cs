using UnityEngine;
using System.Collections;

public class EnemyRotater : MonoBehaviour {

	EnemyController controller;

	public float aggressiveRotateSpeed, calmRotateSpeed;

	void Start () {
		controller = gameObject.GetComponent<EnemyController>();
	}
	
	void Update () {
		if (controller.GetState() != EnemyController.State.Paralyzed) {
			if (controller.CanSeeIt())
				controller.SetState(EnemyController.State.HasSight);
			else
				controller.SetState(EnemyController.State.Idle);
		}

		switch (controller.GetState())
		{
		case EnemyController.State.HasSight:
			Rotate(aggressiveRotateSpeed);
			transform.position = controller.DumbSeek(transform.position, controller.followMe.transform.position);
			break;
		case EnemyController.State.Idle:
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
}
