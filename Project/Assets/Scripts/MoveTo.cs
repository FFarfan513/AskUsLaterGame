using UnityEngine;
using System.Collections;

public class MoveTo : MonoBehaviour {
	private Vector3 currentPosition;
	private Vector3 heading;
	private Vector3 moveHere;
	private bool isMoving;
	public GameObject followMe;
	public float moveSpeed;
	private float bounceDist = 0.15f;

	//rippleSpeed is the speed at which the ripples come out.
	//count counts down by that number and is reset after it reaches 0
	public GameObject ripplePrefab;
	public GameObject destroyRingPrefab;
	private readonly float rippleSpeed = 0.4f;
	private float count;

	void Start () {
		count = 0f;
		currentPosition = transform.position;
	}
	void FixedUpdate() {
		if (count>0)
			count -= Time.deltaTime;
	}
	
	void Update () {
		currentPosition = transform.position;
		moveHere = followMe.transform.position;
		heading = (moveHere - currentPosition).normalized;
		//this check here is so that we're not constantly setting our position to moveHere when we're already there
		if (transform.position != moveHere) {
			//if the distance between us and our target is less than .1, teleport there.
			//this is to prevent that weird shaking that happens a lot.
			if (Vector3.Distance(currentPosition,moveHere) < 0.1f) {
				transform.position = moveHere;
				if (isMoving)
					isMoving = false;
				if (count <= 0.2f)
					Ripple();
			}
			else {
				Vector3 target = heading * moveSpeed + currentPosition;
				transform.position = Vector3.Lerp(currentPosition, target, Time.deltaTime);
				if (!isMoving)
					isMoving = true;
				if (count <= 0) {
					count = rippleSpeed;
					Ripple();
				}
			}
		}
		else {
			if (isMoving) {
				isMoving = false;
			}
		}
	}

	public Vector3 getHeading() {
		return heading;
	}

	void OnCollisionEnter2D(Collision2D other) {
		//this makes us bounce back a bit, so that we can't phase into the wall
		if (other.gameObject.tag == "Wall") {
			transform.position += (-heading*bounceDist);
			followMe.transform.position = transform.position;
		}
	}

	//If we run into an enemy, create the destroyRing around the player.
	//calcuating damage will be done by the enemy.
	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Enemy") {
			var en = other.gameObject.GetComponent<EnemyController>();
			if (en.GetState() != EnemyController.State.Paralyzed)
				DestroyRing();
		}
	}

	void OnCollisionStay2D() {
		//This is so that in case the game thinks we've already "entered" the wall, and it won't trigger the OnCollisionEnter method
		followMe.transform.position = transform.position;
	}
	
	 void Ripple() {
		if (ripplePrefab!=null) {
			GameObject rip = Instantiate(ripplePrefab, transform.position, Quaternion.identity) as GameObject;
			if (this.tag == "PlayerBlack")
				rip.GetComponent<SpriteRenderer>().color = new Color(15/255f,15/255f,15/255f);
		}
	}

	void DestroyRing() {
		if (destroyRingPrefab!=null)
			Instantiate(destroyRingPrefab, transform.position, Quaternion.identity);
	}

	public bool GetIsMoving() {
		return isMoving;
	}

}
