using UnityEngine;
using System.Collections;

public class GoalController : MonoBehaviour {

	private string playerTag;
	private bool goalReached = false;
	private float turnSpeed = 0.25f;
	private float initialTurnSpeed;
	public GameObject wallToRemove;

	public AudioClip unlockSound;
	private AudioSource onGoal;
	private float lowPitch = 1f, highPitch = 1.04f;
	
	void Start() {
		onGoal = this.GetComponent<AudioSource>();
		if (transform.position.x > 0) {
			playerTag = "PlayerWhite";
			onGoal.pan = 0.2f;
		}
		else {
			playerTag = "PlayerBlack";
			onGoal.pan = -0.2f;
		}
	}

	void Update() {
		Rotate();
		/*
		if (Input.GetKeyDown (KeyCode.S))
			goalReached = true;
		*/
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if (other.tag == playerTag) {
			//if goal
			if (wallToRemove == null) {
				goalReached = true;
				initialTurnSpeed = turnSpeed;
				turnSpeed *= 5f;
			}
			//if switch
			else {
				AudioSource.PlayClipAtPoint(unlockSound,transform.position);
				Destroy(wallToRemove);
				Destroy(gameObject);
			}
		}
	}

	//Make the ambient noise as the player stays in the goal circle
	void OnTriggerStay2D( Collider2D other ) {
		if (other.tag == playerTag && !onGoal.isPlaying) {
			onGoal.pitch = Random.Range (lowPitch, highPitch);
			onGoal.Play();
		}
	}
	
	void OnTriggerExit2D( Collider2D other ) {
		if (other.tag == playerTag) {
			goalReached = false;
			turnSpeed = initialTurnSpeed;
		}
	}

	private void Rotate() {
		transform.Rotate(Vector3.forward * turnSpeed * 50 * Time.deltaTime);
	}

	public bool Reached() {
		return goalReached;
	}
	public void Set(bool set) {
		goalReached = set;
	}
}
