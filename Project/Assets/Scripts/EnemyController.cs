using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour {
	public enum State { Idle, NoSight, HasSight, Paralyzed };
	/*
	Idle is the default state where the enemy isn't doing anything.
	NoSight is when the enemy can't see the player.
	HasSight is when the enemy has a clear line of sight towards the player.
	Paralyzed is an enemy that's frozen. It shouldn't move.
	*/
	
	public GameObject followMe;
	public GameObject myParentSpwaner;
	public float moveSpeed;
	private float initialSpeed;
	public float frozenSeconds;

	private SpriteRenderer render;
	private float nodeDetectionRadius = 2.5f;
	private float sightRadius; //how thick our enemy's line of sight is.
	private string playerTag;
	private LevelTransitionController main;

	private State me;
	
	private LayerMask nodeMask;
	private LayerMask ignoreThese;

	private Color originalColor;
	private Color frozenColor = Color.blue;
	private int frozenCountdown;
	public GameObject freezePrefab;

	//This is all for audio
	private GameObject playerObj;
	private AudioSource source;
	public AudioClip freezeSound;
	public AudioClip damageSound;
	public AudioClip killSound;
	private float lowPitch = 0.7f, highPitch = 1.1f;

	void Start() {
		me = State.Idle;
		render = gameObject.GetComponent<SpriteRenderer>();
		sightRadius = render.bounds.extents.magnitude + 0.01f;
		originalColor = render.color;
		initialSpeed = moveSpeed;

		//Each enemy knows which side it's on
		if (transform.position.x < 0) {
			playerTag = "PlayerBlack";
		}
		else {
			playerTag = "PlayerWhite";
		}

		//if this enemy is following the player, then playerObj is the followMe object.
		//if not, it will have to find it.
		if (followMe.tag == playerTag) {
			playerObj = followMe;
		}
		else {
			playerObj = GameObject.FindWithTag(playerTag);
		}

		foreach (Camera c in Camera.allCameras) {
			if (c.gameObject.name == "CameraMiddle") {
				main = c.gameObject.GetComponent<LevelTransitionController>();
				break;
			}
		}

		nodeMask = LayerMask.GetMask("Node");
		ignoreThese = ~LayerMask.GetMask("Node","Enemy","Ignore Raycast");
		source = playerObj.GetComponent<AudioSource>();
	}

	//checks vision by casting a circlecast from the Vector3 from (usually transform.position) towards followMe.
	public bool CanSeeIt(Vector3 from) {
		Vector2 currentPos = new Vector2(from.x, from.y);
		Vector2 followPos = new Vector2(followMe.transform.position.x, followMe.transform.position.y);
		Vector2 dist = followPos-currentPos;
		RaycastHit2D hit = Physics2D.CircleCast(currentPos, sightRadius, dist, dist.magnitude, ignoreThese);
		if ((hit.collider == null) || (hit.collider.name==followMe.name || hit.collider.tag == playerTag))
			return true;
		else
			return false;
	}

	//Finds a node near Vector3 from, that's closest to the Vector3 target
	//If they're the same, you're essentially finding the node closest to that Vector3
	public Collider2D FindNodeNearXClosestToY(Vector3 from, Vector3 target) {
		return FindClosestTo(Physics2D.OverlapCircleAll(from, nodeDetectionRadius, nodeMask), target);
	}

	Collider2D FindClosestTo(Collider2D[] hits, Vector3 target) {
		if (hits.Length <= 0)
			return null;
		Collider2D closest = hits[0];
		//This is actually kind of like a miniature A*.
		if (hits.Length > 1) {
			Vector3 currentPos = transform.position;
			float min = Vector3.Distance(closest.transform.position,currentPos) + Vector3.Distance(closest.transform.position,target);
			for (int i=1; i<hits.Length; i++) {
				float dist = Vector3.Distance(hits[i].transform.position,currentPos) + Vector3.Distance(hits[i].transform.position,target);
				if (dist < min) {
					min = dist;
					closest = hits[i];
				}
			}
		}
		return closest;
	}
	
	
	public List<int> Astar(int startNode, int endNode) {
		HashSet<int> closed = new HashSet<int>();
		HashSet<int> open = new HashSet<int>();
		List <int> adj = new List<int>();
		
		int nodeCount = GenerateGraph.nodes.Count;
		float[] gScores = new float[nodeCount];
		float[] fScores = new float[nodeCount];
		int[] cameFrom = Enumerable.Repeat(-1, nodeCount).ToArray(); //initializes the array with -1's.
		
		int currentNode = startNode;
		
		Vector3 startNodePos, endNodePos, currentNodePos, neighborNodePos;
		GenerateGraph.nodes.TryGetValue (startNode, out startNodePos);
		GenerateGraph.nodes.TryGetValue (endNode, out endNodePos);
		currentNodePos = startNodePos;
		
		//Initialize the openset with the startNode.
		open.Add (startNode);
		gScores [startNode] = 0;
		fScores[startNode] = gScores[startNode] + Vector3.Distance (startNodePos,endNodePos);
		while (open.Count > 0) {
			//the currentNode is the node in the open set with the lowest f score.
			int checkingCurrent = open.ElementAt(0);
			foreach (int n in open) {
				if (fScores[n] < fScores[checkingCurrent])
					checkingCurrent = n;
			}
			currentNode = checkingCurrent;
			
			//if we've reached the end, contsruct our path and end.
			if (currentNode == endNode) {
				return ConstructPath (cameFrom, endNode);
			}
			
			//for each node we look at, remove it from the open set and add it to the closed set.
			open.Remove (currentNode);
			closed.Add (currentNode);
			
			GenerateGraph.graph.TryGetValue(currentNode, out adj);
			foreach (int neigh in adj) {
				if (closed.Contains (neigh)) {
					continue;
				}
				GenerateGraph.nodes.TryGetValue (currentNode, out currentNodePos);
				GenerateGraph.nodes.TryGetValue (neigh, out neighborNodePos);
				float gtemp = gScores[currentNode] + Vector3.Distance(currentNodePos,neighborNodePos);
				
				bool notInOpenSet = !open.Contains(neigh);
				if (notInOpenSet || ( gtemp < gScores[neigh])) {
					cameFrom[neigh] = currentNode;
					gScores[neigh] = gtemp;
					fScores[neigh] = gScores[neigh] + Vector3.Distance (neighborNodePos,endNodePos);
					if (notInOpenSet)
						open.Add (neigh);
				}
			}
		}
		//Under normal circumstances, this shouldn't happen.
		Debug.Log ("ERROR! An enemy can't move using A*! Perhaps the start and end nodes of the path are the cause.\n");
		return (new List<int>());
	}
	
	List<int> ConstructPath(int[] cameFrom, int current) {
		List<int> totalPath = new List<int>();
		totalPath.Add (current);
		while (cameFrom[current] != -1) {
			current = cameFrom[current];
			totalPath.Add(current);
		}
		//note that the path is backwards, so the first node to move to is the one at the end
		return totalPath;
	}

	//To use this, you do object.transform.position = DumbSeek(whatever);
	public Vector3 DumbSeek (Vector3 moveFrom, Vector3 moveHere) {
		float distanceBetween = Vector3.Distance(moveFrom,moveHere);
		if (distanceBetween < 0.1f) {
			return moveHere;
		}
		else {
			Vector3 moveDirection = moveHere - moveFrom;
			moveDirection.z = 0;
			moveDirection.Normalize();
			
			Vector3 target = moveDirection * moveSpeed + moveFrom;
			return Vector3.Lerp(moveFrom, target, Time.deltaTime);
		}
	}

	//plays the freeze animation and freezes this enemy.
	public void Neutralize() {
		Instantiate(freezePrefab, transform.position, transform.rotation);
		frozenCountdown = Mathf.FloorToInt(frozenSeconds*60);
		me = State.Paralyzed;
		render.color = frozenColor;
		PlaySound(freezeSound);
	}

	//if this enemy is paralyzed, start a countdown. after it's up, unfreeze this enemy.
	void FixedUpdate() {
		if (me == State.Paralyzed && frozenCountdown > 0) {
			frozenCountdown--;
			if (frozenCountdown <= 60) {
				if (frozenCountdown%10 == 0)
					UnfreezingAnim();
			}
			if (frozenCountdown <= 0) {
				UnFreeze();
			}
		}
	}

	void UnFreeze() {
		//Everytime you unfreeze, it sets your state to Idle.
		me = State.Idle;
		this.GetComponent<SpriteRenderer>().color = originalColor;
		frozenCountdown = 0;
		moveSpeed = initialSpeed;
	}

	//Essentially what I'm doing is that after a specific interval, it will switch it's color
	//to a darker blue shade, and back. It'll flash three times before unfreezing.
	void UnfreezingAnim() {
		if (render.color == frozenColor) {
			render.color = new Color(0f,0f,135/255.0f);}
		else
			render.color = frozenColor;
	}
	
	public void KillMe() {
		//if this enemy is not paralyzed it creates the damage animation (freeze animation with the red color)
		if (me != State.Paralyzed) {
			GameObject fr = Instantiate(freezePrefab, transform.position, Quaternion.identity) as GameObject;
			fr.transform.rotation = this.transform.rotation;
			fr.GetComponent<SpriteRenderer>().color = originalColor;
		}
		//if this enemy is paralyzed, it recreates the freeze animation 
		else {
			GameObject fr = Instantiate(freezePrefab, transform.position, Quaternion.identity) as GameObject;
			fr.transform.rotation = this.transform.rotation;
		}

		if (myParentSpwaner != null)
			myParentSpwaner.GetComponent<EnemySpawnerController>().DecrementChildren();
		DestroyObject(gameObject);
	}

	//Called by other objects to inflict damage to the player.
	public void Damage() {
		//if this enemy is not paralyzed it decrements the player's HP and plays the damageSound
		if (me != State.Paralyzed) {
			PlaySound(damageSound);
			main.DecrementHP();
		}
		//if this enemy is paralyzed, it plays the killSound
		else {
			PlaySound(killSound);
		}
		//The destruction of the object goes in the enemy specific scripts.
	}

	public void PlaySound(AudioClip clip) {
		if (source != null) {
			source.pitch = Random.Range (lowPitch, highPitch);
			source.PlayOneShot(clip);
		}
	}

	//Getters/Setters:
	public State GetState() {
		return me;
	}
	public void SetState(State newState) {
		me = newState;
	}
	public string GetPlayerTag() {
		return playerTag;
	}
}