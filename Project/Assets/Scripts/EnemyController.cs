using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyController : MonoBehaviour {
	public enum State { Searching, Pathing, HasSight, Paralyzed };
	/*
	Searching is the default state where the enemy isn't doing anything.
	Pathing is when the enemy is trying to move to the player but can't see them.
	HasSight is when the enemy has a clear line of sight towards the player.
	Paralyzed is an enemy that's frozen. It shouldn't move.
	*/
	
	public GameObject followMe;
	public GameObject myParentSpwaner;
	public float moveSpeed;
	public int deadlyMouseButton;
	public int frozenSeconds;
	public float nodeDetectionRadius = 2.5f;
	private float sightRadius; //how thick our enemy's line of sight is.
	public string playerTag;
	
	private List<int> path;
	private State me;
	
	public LayerMask nodeMask;
	public LayerMask nodeAndEnemy;
	private Color frozenColor = Color.blue; //for now. Later I'll input actual RGBA values.
	
	void Start() {
		path = new List<int>();
		me = State.Searching;
		SpriteRenderer s = renderer as SpriteRenderer;
		sightRadius = s.bounds.extents.magnitude + 0.05f;
		if (deadlyMouseButton == 1)
			playerTag = "PlayerBlack";
		else if (deadlyMouseButton == 0)
			playerTag = "PlayerWhite";
	}
	
	void Update() {
		if (me != State.Paralyzed) {
			Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
			Vector2 followPos = new Vector2(followMe.transform.position.x, followMe.transform.position.y);
			Vector2 dist = followPos-currentPos;
			RaycastHit2D hit = Physics2D.CircleCast(currentPos, sightRadius, dist, dist.magnitude, ~nodeAndEnemy);
			//Debug.DrawRay (currentPos,dist,Color.red);
			if ((hit.collider == null) || (hit.collider.name==followMe.name || hit.collider.tag == playerTag))
				me = State.HasSight;
			else
				me = State.Pathing;
		}
		switch (me) {
		case State.HasSight:
			path.Clear ();
			DumbSeek(followMe.transform.position);
			break;
		case State.Pathing:
			int youMoved = -1, start, end;
			Collider2D myCircle =
				FindClosestTo(Physics2D.OverlapCircleAll(transform.position, nodeDetectionRadius, nodeMask), followMe.transform.position);
			Collider2D targetCircle =
				FindClosestTo(Physics2D.OverlapCircleAll(followMe.transform.position, nodeDetectionRadius, nodeMask), followMe.transform.position);
			//if there is no path yet
			if (myCircle!=null && targetCircle!= null) {
				if (path.Count == 0) {
					if (int.TryParse(myCircle.name, out start) && int.TryParse(targetCircle.name, out end)) {
						path = Astar(start, end);
					}
				}
				//else if closest node to player has changed
				else if (int.TryParse(targetCircle.name, out youMoved) && (youMoved != path[0]) && !path.Contains (youMoved)) {
					path = Astar(path[path.Count-1], youMoved);
				}
				MoveThroughPath();
				//PrintPath(path);
			}
			break;
		case State.Paralyzed:
			break;
		default:
			print("ERROR! The state was somehow set to something other than it's enum values.\n");
			break;
		}
	}
	
	//This can actually be used for things other than just finding the closest node to an object
	//It takes in a list of colliders and returns the collider closest to a target position
	Collider2D FindClosestTo(Collider2D[] hits, Vector3 target) {
		if (hits.Length <= 0)
			return null;
		Collider2D closest = hits[0];
		if (hits.Length > 1) {
			float min = Vector3.Distance (closest.transform.position, target);
			for (int i=1; i<hits.Length; i++) {
				float dist = Vector3.Distance (hits[i].transform.position, target);
				if (dist < min) {
					min = dist;
					closest = hits[i];
				}
			}
		}
		return closest;
	}
	
	
	List<int> Astar(int startNode, int endNode) {
		HashSet<int> closed = new HashSet<int>();
		HashSet<int> open = new HashSet<int>();
		List <int> adj = new List<int>();
		
		////int nodeCount = GenerateGraph.nodeVectors.Count;
		int nodeCount = GenerateGraph.nodes.Count;
		float[] gScores = new float[nodeCount];
		float[] fScores = new float[nodeCount];
		int[] cameFrom = Enumerable.Repeat(-1, nodeCount).ToArray(); //initializes the array with -1's.
		
		int currentNode = startNode;
		
		Vector3 startNodePos, endNodePos, currentNodePos, neighborNodePos;
		////startNodePos = GenerateGraph.nodeVectors[startNode];
		////endNodePos = GenerateGraph.nodeVectors[endNode];
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
				////currentNodePos = GenerateGraph.nodeVectors[currentNode];
				////neighborNodePos = GenerateGraph.nodeVectors[neigh];
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
		Debug.Log ("I can't reach there using A*! Perhaps the start and end nodes of the path are the cause.\n");
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
	
	void DumbSeek (Vector3 moveHere) {
		Vector3 currentPosition = transform.position;
		float distanceBetween = Vector3.Distance(currentPosition,moveHere);
		if (distanceBetween < 0.1f) {
			transform.position = moveHere;
		}
		else {
			Vector3 moveDirection = moveHere - currentPosition;
			moveDirection.z = 0;
			moveDirection.Normalize();
			
			Vector3 target = moveDirection * moveSpeed + currentPosition;
			transform.position = Vector3.Lerp(currentPosition, target, Time.deltaTime);
		}
	}
	
	void MoveThroughPath() {
		//this function always moves to the last node in the path, and removes it when it reaches it.
		if (path.Count>0) {
			int step = path[path.Count-1];
			////Vector3 dest = GenerateGraph.nodeVectors[step];
			Vector3 dest;
			GenerateGraph.nodes.TryGetValue (step, out dest);
			DumbSeek(dest);
			if (Vector3.Distance(transform.position, dest) < 0.1f)
				path.RemoveAt(path.Count-1);
		}
	}
	
	void PrintPath(List<int> thispath) {
		string p = "";
		foreach (int n in thispath) {
			p = p + n + ", ";
		}
		if (p.Length > 2) {
			Debug.Log (p.Substring (0,p.Length-2) + "\n");
		}
	}
	
	void OnMouseOver() {
		if(Input.GetMouseButtonDown(deadlyMouseButton) && me != State.Paralyzed) {
			me = State.Paralyzed;
			Color originalColor = this.GetComponent<SpriteRenderer>().color;
			this.GetComponent<SpriteRenderer>().color = frozenColor;
			StartCoroutine(Frozen(originalColor));
		}
	}
	
	IEnumerator Frozen(Color originalColor) {
		yield return new WaitForSeconds(frozenSeconds);
		me = State.Searching;
		this.GetComponent<SpriteRenderer>().color = originalColor;
	}
	
	void OnTriggerEnter2D( Collider2D other ) {
		if (other.tag == playerTag) {
			if (me != State.Paralyzed) {
				//Player takes damage!
				print("Damage!\n");
			}
			KillMe();
		}
	}
	
	void KillMe() {
		if (myParentSpwaner != null)
			myParentSpwaner.GetComponent<EnemySpawnerController>().childrenSpawned--;
		DestroyObject(gameObject);
	}
}