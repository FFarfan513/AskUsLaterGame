using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour {

	public float turnSpeed;

	// Update is called once per frame
	void Update () {
		Rotate();
	}

	void Rotate() {
		transform.Rotate(Vector3.forward * turnSpeed * 50 * Time.deltaTime);
	}
}
