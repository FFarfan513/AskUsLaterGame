using UnityEngine;
using System.Collections;

public class BarrierController : MonoBehaviour {
	public GameObject barrierPiecePrefab;
	private float y;

	void Start () {
		//Create BarrierPiece objects along the y scale of this Barrier
		//All created BarrierPieces are children to this Barrier, so when the barrier is destroyed, so are the pieces.
		y = transform.localScale.y;
		GameObject child = Instantiate(barrierPiecePrefab, transform.position, Quaternion.identity) as GameObject;
		child.transform.parent = transform;
		//this is a similar process to the HealthBar creation.
		for (float i=1.5f; i < y; i+=1.5f) {
			GameObject ch = Instantiate(barrierPiecePrefab, new Vector3(transform.position.x,transform.position.y+i), Quaternion.identity) as GameObject;
			GameObject ild = Instantiate(barrierPiecePrefab, new Vector3(transform.position.x,transform.position.y-i), Quaternion.identity) as GameObject;
			ch.transform.parent = transform;
			ild.transform.parent = transform;
		}
	}
}
