using UnityEngine;
using System.Collections;

public class EnemySpawnerController : MonoBehaviour {

	public float chanceToSpawn;		// Percent chance needed to spawn per Update (between 0 and 100)
	public float myMaxRangeToSpawn;	// Maximum distance to player to allow spawning
	public float myMinRangeToSpawn;	// Minimum distance to player to allow spawning
	private float myChance;			// Used to determine when to spawn
	private float myDistance;		// Current distance from this spawner to the player
	public int mySide;				// 0 == right side, 1 == left side
	public int maxCanSpawn;			// Maximum number of enemies this spawner can spawn
	public int childrenSpawned;		// Current number of enemies this spawner has spawned
	public int noSpawnTime;			// Time the spawner waits after spawning before considering spawning again
	private int myCountdown;		// Time left until spawner can consider spawning again
	private GameObject myPlayer;	// White or Black Player GameObject
	public GameObject enemyPrefab;	// Type of enemy this spawner creates
	private string myPlayerTag;		// Tag name of player

	void Start() {
		childrenSpawned = 0;
		if (mySide == 1){
			myPlayerTag = "PlayerBlack";
		}
		else {
			myPlayerTag = "PlayerWhite";
		}
		
		myPlayer = GameObject.FindGameObjectWithTag(myPlayerTag);
	}
	
	// Update is called once per frame
	void Update () {
	
		// Recalculate distance to player every Update
		myDistance = Vector3.Distance(transform.position, myPlayer.transform.position);
		//Debug.Log(myDistance);

		// Decrease noSpawnTime every Update
		if (myCountdown > 0) {
			myCountdown--;
			Debug.Log(myCountdown);
		}

		// If player is within range, and spawned less than max children, and not on cooldown
		if (myDistance <= myMaxRangeToSpawn && myDistance >= myMinRangeToSpawn
		    && childrenSpawned < maxCanSpawn && myCountdown == 0) {

			// Roll the dice
			myChance = Random.Range(0f, 100f);

			// If chance randomly wants to spawn, spawn an enemy
			if (myChance <= chanceToSpawn) {
				myCountdown = noSpawnTime;
				//Debug.Log("SPAWNED");
				childrenSpawned++;
				GameObject child = (GameObject)Instantiate(enemyPrefab, transform.position, Quaternion.identity);
				child.GetComponent<EnemyController>().myParentSpwaner = transform.gameObject;
				child.GetComponent<EnemyController>().deadlyMouseButton = mySide;
				child.GetComponent<EnemyController>().playerTag = myPlayerTag;
				child.GetComponent<EnemyController>().followMe = myPlayer;
			}
		}

	} // end of Update()

}
