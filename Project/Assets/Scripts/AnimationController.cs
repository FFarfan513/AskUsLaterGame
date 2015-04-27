using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {
	public void Delete() {
		Destroy( this.gameObject );
	}
	
	void OnTriggerEnter2D( Collider2D other ) {
		//if this animation is attached to the DestroyRing, and it collides with an enemy,
		//call the KillMe function on those enemies to destroy them.
		if (this.tag == "DestroyRing") {
			if (other.tag == "Enemy")
				other.GetComponent<EnemyController>().KillMe();
		}
	}
}
