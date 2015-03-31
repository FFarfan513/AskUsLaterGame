using UnityEngine;
using System.Collections;

public class EnemySpawnerController : MonoBehaviour {

	public float chanceToSpawn;		// Percent chance needed to spawn per Update (between 0 and 100)
	public float myMaxRangeToSpawn; // Maximum distance to player to allow spawning
	private float myChance;			// Used to determine when to spawn
	private float myDistance;		// Current distance from this spawner to the player
	private int myDeadlyButton;		// 0 == Killed by left click, 1 == Killed by right click
	public int maxCanSpawn;			// Maximum number of enemies this spawner can spawn
	private int childrenSpawned;	// Current number of enemies this spawner has spawned
	public int noSpawnTime;			// Time the spawner waits after spawning before considering spawning again
	private int myCountdown;		// Time left until spawner can consider spawning again
	
	public GameObject followThis;	// Which gameObject to follow
	public GameObject enemyPrefab;	// Type of enemy this spawner creates
	public float myMoveSpeed;		// The (initial) move speed of the objects we spawn
	public float myFrozenSeconds;	// The amount of time an object spends frozen

	private float cameraLeniency = 0.2f;
	private Camera cam;
	//private float idleConstant = 1;

	void Start() {
		childrenSpawned = 0;
		if (transform.position.x < 0) {
			myDeadlyButton = 1;
			cam = Camera.main;
		}
		else {
			myDeadlyButton = 0;
			foreach (Camera c in Camera.allCameras) {
				if (c.gameObject.name == "CameraWhite") {
					cam = c;
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		// Recalculate distance to player every Update
		myDistance = Vector3.Distance(transform.position, followThis.transform.position);
		//Debug.Log(myDistance);

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
				/*if (this side is idle for too long) {
					Spawn(myMoveSpeed + idleConstant++);
					myChance to spawn goes up
				}
				else */
					Spawn(myMoveSpeed);
			}
		}

	} // end of Update()

	//Regular spawn
	void Spawn(float speed) {
		GameObject child = (GameObject)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
		Init(child.GetComponent<EnemyController>(), speed);
	}

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
		c.deadlyMouseButton = myDeadlyButton;
		c.moveSpeed = speed;
		c.followMe = followThis;
		c.frozenSeconds = myFrozenSeconds;
	}

	bool NotNearScreen() {
		Vector3 relativePos = cam.WorldToViewportPoint(transform.position);
		if (relativePos.x>(1+cameraLeniency) || relativePos.x<(-cameraLeniency) ||
		    relativePos.y>(1+cameraLeniency) || relativePos.y<(-cameraLeniency))
			return true;
		else
			return false;
	}

	public void DecrementChildren() {
		childrenSpawned--;
	}
	
}
