using UnityEngine;
using System.Collections;

public class GoalController : MonoBehaviour {

	private GameObject player;
	private bool goalReached = false;
	public float turnSpeed;
	public GameObject wallToRemove;

	public AudioClip unlockSound;
	private AudioSource onGoal;
	private float lowPitch = 1f, highPitch = 1.04f;
	
	void Start() {
		player = FindPlayer();
		onGoal = this.GetComponent<AudioSource>();
		if (transform.position.x > 0)
			onGoal.pan = 0.2f;
		else
			onGoal.pan = -0.2f;
	}

	void Update() {
		Rotate();
		//Let's us skip levels for testing transitions by pressing S
		if (Input.GetKeyDown (KeyCode.S))
			goalReached = true;

	}

	void OnTriggerEnter2D( Collider2D other ) {
		if (other.tag == player.tag) {
			//if goal
			if (wallToRemove == null) {
				goalReached = true;
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
		if (other.tag == player.tag && !onGoal.isPlaying) {
			onGoal.pitch = Random.Range (lowPitch, highPitch);
			onGoal.Play();
		}
	}
	
	void OnTriggerExit2D( Collider2D other ) {
		if (other.tag == player.tag) {
			goalReached = false;
			turnSpeed /= 5f;
		}
	}

	private void Rotate() {
		transform.Rotate(Vector3.forward * turnSpeed * 50 * Time.deltaTime);
	}

	private GameObject FindPlayer() {
		if (transform.position.x < 0)
			return GameObject.FindWithTag("PlayerBlack");
		else
			return GameObject.FindWithTag("PlayerWhite");
	}

	public bool Reached() {
		return goalReached;
	}
	public void Reset() {
		goalReached = false;
	}
}
