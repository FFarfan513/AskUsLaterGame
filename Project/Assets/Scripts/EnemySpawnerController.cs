using UnityEngine;
using System.Collections;

public class EnemySpawnerController : MonoBehaviour {

	public int chanceToSpawn;		// Percent chance needed to spawn per Update (between 0 and 100)
	public float myRangeToSpawn;	// Minimum distance to player to allow spawning
	private int myChance;			// Used to determine when to spawn
	private float myDistance;		// Current distance from this spawner to the player
	public int mySide;				// 0 == right side, 1 == left side
	public int maxCanSpawn;			// Maximum number of enemies this spawner can spawn
	private int childrenSpawned;	// Current number of enemies this spawner has spawned
	private GameObject myPlayer;	// White or Black Player GameObject
	public GameObject enemyPrefab;	// Type of enemy this spawner creates

	void Start() {
		childrenSpawned = 0;
		if (mySide == 0)
			myPlayer = GameObject.FindGameObjectWithTag("PlayerBlack");
		else
			myPlayer = GameObject.FindGameObjectWithTag("PlayerWhite");
	}
	
	// Update is called once per frame
	void Update () {
	
		// Recalculate distance to player every Update
		myDistance = Vector3.Distance(transform.position, myPlayer.transform.position);

		// If player is within range, and spawned less than max children
		if (myDistance <= myRangeToSpawn && childrenSpawned < maxCanSpawn) {

			// Roll the dice
			myChance = Random.Range(0, 100);

			// If chance randomly wants to spawn, spawn an enemy
			if (myChance <= chanceToSpawn) {
				childrenSpawned++;
				Instantiate(enemyPrefab, transform.position, Quaternion.identity);
			}
		}

	} // end of Update()

}
