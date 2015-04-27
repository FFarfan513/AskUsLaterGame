using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdleController : MonoBehaviour {
	private Camera cam;
	private AudioSource intro;
	private AudioSource loop;
	//musicVolume should be the same for both intro and loop.
	private float musicVolume;

	private List<RaycastHit2D[]> edges;

	private bool isIdle;
	private int idleCount;
	public GameObject fog;
	private SpriteRenderer fogColor;
	private float fogSpeed = 3/255.0f;
	private int waitTime = 130;
	private int spawnTime = 110;
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
		Camera mid = null;
		foreach (Camera c in Camera.allCameras) {
			if (c.gameObject.name == "CameraMiddle") {
				mid = c;
				break;
			}
		}
		intro = mid.GetComponent<PlayMusic>().intro;
		loop = mid.GetComponent<PlayMusic>().loop;
		musicVolume = loop.volume;
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
			if (idleCount == waitTime+4) {
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
				if (intro != null) {
					intro.pan = 0f;
					intro.volume = musicVolume;
				}
				loop.pan = 0f;
				loop.volume = musicVolume;
			}
		}
	}

	//This is what ClickToMove calls to set the isIdle to true or false.
	//if it's false, idlecount is set to 0.
	public void IsIdling(bool id) {
		isIdle = id;
		if (!id) {
			idleCount = 0;
		}
	}
	public bool GetIsIdle() {
		return isIdle;
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
		//Makes the music pan away from the side that's idling
		if (loop.isPlaying) {
			ShiftMusic(loop);
		}
		else if (intro != null && intro.isPlaying) {
			ShiftMusic(intro);
		}
		//the fogColor's alpha increases by fogSpeed
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
	
	void ShiftMusic(AudioSource source) {
		//if PlayerWhite is idling, pan the music towards the left.
		if (player.transform.position.x > 0) {
			//if the music is already panned towards the right (in that PlayerBlack has idled before PlayerWhite),
			//lower the volume instead.
			if (source.pan > 0)
				source.volume = Mathf.Round((source.volume-.005f) * 1000f) / 1000f;
			else
				source.pan = Mathf.Round((source.pan-fogSpeed) * 100f) / 100f;
			}
		//if PlayerBlack is idling pan the music towards the right.
		else {
			//if the music is already panned towards the left (in that PlayerWhite has idled before PlayerBlack),
			//lower the volume instead.
			if (source.pan < 0)
				source.volume = Mathf.Round((source.volume-.005f) * 1000f) / 1000f;
			else
				source.pan = Mathf.Round((source.pan+fogSpeed) * 100f) / 100f;
		}
	}

	//adds +2 to each visible enemy's speed.
	void SpeedUpVisibleEnemies() {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
		foreach (var en in enemies) {
			if (en.collider2D != null && GeometryUtility.TestPlanesAABB(planes, en.collider2D.bounds) )
				en.GetComponent<EnemyController>().moveSpeed += 2;
		}
	}

	//spawns enemies at nodes located at the edges of the screen if available
	void SpawnEnemies() {
		Vector3 botLeft  = cam.ViewportToWorldPoint(new Vector3(0.1f,0.1f,CenterOn.cameraZ));
		Vector3 topLeft  = cam.ViewportToWorldPoint(new Vector3(0.1f,0.9f,CenterOn.cameraZ));
		Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(0.9f,0.9f,CenterOn.cameraZ));
		Vector3 botRight = cam.ViewportToWorldPoint(new Vector3(0.9f,0.1f,CenterOn.cameraZ));

		edges = new List<RaycastHit2D[]>(4);
		edges.Add (Physics2D.CircleCastAll(botLeft, 1f,(botRight-botLeft), (botRight-botLeft).magnitude, n));
		edges.Add (Physics2D.CircleCastAll(botLeft, 1f,(topLeft-botLeft),  (topLeft-botLeft).magnitude,  n));
		edges.Add (Physics2D.CircleCastAll(topRight,1f,(topLeft-topRight), (topLeft-topRight).magnitude, n));
		edges.Add (Physics2D.CircleCastAll(topRight,1f,(botRight-topRight),(botRight-topRight).magnitude,n));

		//for each screen edge, choose a random node along that edge and spawn an enemy there.
		foreach (var edge in edges) {
			if (edge.Length > 0) {
				int r = Mathf.FloorToInt(Random.Range (0,edge.Length));
				Vector3 pos = edge[r].transform.position;
				GameObject child = (GameObject)Instantiate(enemyPrefab, pos, Quaternion.identity);
				var c = child.GetComponent<EnemyController>();
				c.moveSpeed = 2;
				c.followMe = player;
				c.frozenSeconds = 3;
			}
		}
	}
}

