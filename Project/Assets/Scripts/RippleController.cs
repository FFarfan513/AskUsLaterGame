using UnityEngine;
using System.Collections;

public class RippleController : MonoBehaviour {

	public void myDelete() {
		Destroy( this.gameObject );
	}
}
