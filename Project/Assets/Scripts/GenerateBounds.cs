using UnityEngine;
using System.Collections;

public class GenerateBounds : MonoBehaviour {

	public float distanceToCenter, offset;
	public GameObject wallPrefab;

	void Update() {
		if (Input.GetKeyDown(KeyCode.A)) { // obviously, this will not be the condition for a new level
			newLevel();
		}
	}


	/**
	 * Create new level for both players
	 */
	public void newLevel() {

		GameObject pw, pb;

		wipeWalls();

		pw = GameObject.FindGameObjectWithTag("PlayerWhite");
		pb = GameObject.FindGameObjectWithTag("PlayerBlack");

		createLevel(pw.transform.position);
		createLevel(pb.transform.position);

	} // end of newLevel()


	/**
	 * Given player position, create circular level boundary
	 */
	void createLevel(Vector3 pos) {

		Vector3 center, wallPos;
		int direction;
		float cx, cy;

		// Randomly pick location around 
		// the circle that the player starts at
		direction = Random.Range(0, 360);

		// Determine where the center position is
		// based on where the player starts
		cx = Mathf.Sin(direction) * distanceToCenter;
		cy = Mathf.Cos(direction) * distanceToCenter;
		center = new Vector3(pos.x + cx, pos.y + cy, 0);

		// Create 360 walls around center position
		for (int i = 0; i < 360; i++) {
			cx = Mathf.Cos(i) * (distanceToCenter + offset);
			cy = Mathf.Sin(i) * (distanceToCenter + offset);
			wallPos = new Vector3(cx + center.x, cy + center.y, 0);
			Instantiate(wallPrefab, wallPos, Quaternion.Euler(0,0,(float)i));
		}

	} // end of createLevel()


	/**
	 * Before creating new level, destroy all walls leftover, if any
	 */
	public void wipeWalls() {
		GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
		for (int i = 0; i < walls.Length; i++) {
			DestroyObject( walls[i] );
		}
	} // end of wipeWalls()

} // end of script
