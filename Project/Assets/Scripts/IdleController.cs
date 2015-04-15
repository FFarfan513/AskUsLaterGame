using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdleController : MonoBehaviour {
	private Camera cam;
	private bool isIdle;
	private int idleCount;
	public GameObject fog;
	private SpriteRenderer fogColor;
	private float fogSpeed = 3/255.0f;
	private int waitTime = 130;
	private int spawnTime = 80;
	private int coolDown;

	//prevents the idle check until the player clicks at least once
	private bool levelStart;
	public GameObject enemyPrefab;
	private GameObject player;
	private LayerMask n;

	void Start () {
		coolDown = spawnTime;
		cam = this.camera;
		player = this.GetComponent<CenterOn>().character;
		fogColor = fog.GetComponent<SpriteRenderer>();
		n = LayerMask.GetMask("Node");
	}
	
	void FixedUpdate () {
		if (levelStart) {
			//if the player is idle, increment the idle count.
			if (isIdle) {
				idleCount++;
			}
			//once the idle time hits a certain point, we speed up the enemies on this side of the screen.
			if (idleCount == waitTime+1) {
				SpeedUpVisibleEnemies();
			}
			//If we're idling past this point, fog up our screen.
			if (idleCount > waitTime) {
				fogColor.color = FogUp();
				//Also implemented a spawning and cooldown system similar to
				//the one in EnemySpawnerController
				if (coolDown == spawnTime && fogColor.color.a>=1) {
					SpawnEnemies();
					coolDown = 0;
				}
				if(coolDown != spawnTime)
					coolDown++;
			}
			//if the idleCount has been reset and the screen isn't clear, clear it.
			if (idleCount < waitTime && fogColor.color.a!=0) {
				coolDown = spawnTime;
				Color temp = fogColor.color;
				temp.a = 0;
				fogColor.color = temp;
			}
		}
	}

	//This is what ClickToMove calls to set the isIdle to true or false.
	//if it's false, idlecount is set to 0.
	public void IsIdling(bool id) {
		isIdle = id;
		if (!id)
			idleCount = 0;
	}

	//Lets us know that the first click has happened and to start checking for idling.
	public void Go() {
		levelStart = true;
	}
	//let's other scripts know we've started.
	public bool Started() {
		return levelStart;
	}

	//Fogs up the screen by raising the fog object's alpha slowly.
	Color FogUp() {
		Color temp = fogColor.color;
		float alpha = temp.a;
		if (alpha >= 1.0) {
			temp.a = 1;
			return temp;
		}
		else {
			temp.a += fogSpeed;
			return temp;
		}
	}

	//adds +2 to each visible enemy's speed. Can change this.
	void SpeedUpVisibleEnemies() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
		foreach (var en in enemies) {
			if ( GeometryUtility.TestPlanesAABB(planes, en.collider2D.bounds) )
				en.GetComponent<EnemyController>().moveSpeed += 2;
		}
	}

	//Not 100% finished yet. I sometimes get strange errors.
	void SpawnEnemies() {
		Vector3 bottomLeft  = cam.ViewportToWorldPoint(new Vector3(0.1f,0.1f,CenterOn.cameraZ));
		Vector3 topLeft     = cam.ViewportToWorldPoint(new Vector3(0.1f,0.9f,CenterOn.cameraZ));
		Vector3 topRight    = cam.ViewportToWorldPoint(new Vector3(0.9f,0.9f,CenterOn.cameraZ));
		Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(0.9f,0.1f,CenterOn.cameraZ));

		List<RaycastHit2D[]> edges = new List<RaycastHit2D[]>();
		edges.Add (Physics2D.CircleCastAll (bottomLeft,1f,(bottomRight-bottomLeft),(bottomRight-bottomLeft).magnitude,n));
		edges.Add (Physics2D.CircleCastAll (bottomLeft,1f,(topLeft-bottomLeft),(topLeft-bottomLeft).magnitude,n));
		edges.Add (Physics2D.CircleCastAll (topRight,1f,(topLeft-topRight),(topLeft-topRight).magnitude,n));
		edges.Add (Physics2D.CircleCastAll (topRight,1f,(bottomRight-topRight),(bottomRight-topRight).magnitude,n));

		foreach (var edge in edges) {
			if (edge.Length > 0) {
				int r = Mathf.FloorToInt(Random.Range (0,edge.Length));
				Vector3 pos = edge[r].transform.position;
				if (Vector3.Distance (pos,player.transform.position) > 5.0f) {
					GameObject child = (GameObject)Instantiate(enemyPrefab, pos, Quaternion.identity);
					var c = child.GetComponent<EnemyController>();
					c.moveSpeed = 2;
					c.followMe = player;
					c.frozenSeconds = 3;
				}
			}
		}
	}
}

