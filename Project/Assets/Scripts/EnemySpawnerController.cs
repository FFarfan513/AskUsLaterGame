using UnityEngine;
using System.Collections;

public class EnemySpawnerController : MonoBehaviour {

	public float chanceToSpawn;		// Percent chance needed to spawn per Update (between 0 and 100)
	public float myMaxRangeToSpawn; // Maximum distance to player to allow spawning
	private float myChance;			// Used to determine when to spawn
	private float myDistance;		// Current distance from this spawner to the player
	public int maxCanSpawn;			// Maximum number of enemies this spawner can spawn
	private int childrenSpawned;	// Current number of enemies this spawner has spawned
	public int noSpawnTime;			// Time the spawner waits after spawning before considering spawning again
	private int myCountdown;		// Time left until spawner can consider spawning again
	
	public GameObject followThis;	// Which gameObject to follow
	public GameObject enemyPrefab;	// Type of enemy this spawner creates
	public float myMoveSpeed;		// The (initial) move speed of the objects we spawn
	public float myFrozenSeconds;	// The amount of time an object spends frozen

	private float cameraLeniency = 0.05f;
	private Camera cam;

	void Start() {
		childrenSpawned = 0;
		if (transform.position.x < 0) {
			cam = Camera.main;
		}
		else {
			foreach (Camera c in Camera.allCameras) {
				if (c.gameObject.name == "CameraWhite") {
					cam = c;
					break;
				}
			}
		}

		transform.localScale = new Vector3(myMaxRangeToSpawn+3,myMaxRangeToSpawn+3,0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Z)) {
			SpriteRenderer s = gameObject.GetComponent<SpriteRenderer>();
			s.enabled = !s.enabled;
		}
	
		// Recalculate distance to player every Update
		myDistance = Vector3.Distance(transform.position, followThis.transform.position);

		// Decrease noSpawnTime every Update
		if (myCountdown > 0) {
			myCountdown--;
		}

		// If player is within range, and spawned less than max children, and not on cooldown
		if (myDistance <= myMaxRangeToSpawn && NotNearScreen() &&
		    childrenSpawned < maxCanSpawn && myCountdown == 0) {

			// Roll the dice
			myChance = Random.Range(0f, 100f);
			// If chance randomly wants to spawn, spawn an enemy
			if (myChance <= chanceToSpawn) {
				myCountdown = noSpawnTime;
				childrenSpawned++;
				Spawn(myMoveSpeed);
			}
		}

	} // end of Update()

	//Regular spawn
	void Spawn(float speed) {
		GameObject child = (GameObject)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
		Init(child.GetComponent<EnemyController>(), speed);
	}

	//Irregular spawns. We might not end up needing/using these:

	//Spawns enemies with the same sprite as the original, but either scaled larger or smaller
	void SpawnScaled(float speed, float scaling) {
		GameObject child = (GameObject)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
		child.transform.localScale = new Vector3(scaling,scaling,1);
		Init(child.GetComponent<EnemyController>(), speed);
	}

	//Spawns enemies with the same behavior, but a different sprite
	void SpawnDifferent(float speed, Sprite different) {
		GameObject child = (GameObject)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
		child.GetComponent<SpriteRenderer>().sprite = different;
		Init(child.GetComponent<EnemyController>(), speed);
	}

	//This is the part that's shared between all the different spawnings
	void Init(EnemyController c, float speed) {
		c.myParentSpwaner = transform.gameObject;
		c.moveSpeed = speed;
		c.followMe = followThis;
		c.frozenSeconds = myFrozenSeconds;
	}

	bool NotNearScreen() {
		//The bottom-left of the camera is (0,0); the top-right is (1,1).
		Vector3 relativePos = cam.WorldToViewportPoint(transform.position);
		return (relativePos.x>(1+cameraLeniency) || relativePos.x<(-cameraLeniency) ||
		        relativePos.y>(1+cameraLeniency) || relativePos.y<(-cameraLeniency));
	}

	public void DecrementChildren() {
		childrenSpawned--;
	}

}
