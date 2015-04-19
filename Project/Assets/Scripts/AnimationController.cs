using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
	public void Delete() {
		Destroy( this.gameObject );
	}
	
	void OnTriggerEnter2D( Collider2D other ) {
		if (this.tag == "DestroyRing") {
			if (other.tag == "Enemy")
				other.GetComponent<EnemyController>().KillMe();
		}
	}
}
