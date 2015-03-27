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
	public float moveSpeed;
	public int deadlyMouseButton;
	public int frozenSeconds;
	private int colliderSize = 2; //should always be GenerateGraph's spacing - 1
	private string playerTag;
	
	private List<int> path;
	private State me;
	
	public LayerMask nodeMask;
	public LayerMask nodeAndEnemy;
	private Color frozenColor = Color.blue; //for now. Later I'll input actual RGBA values.
	
	void Start() {
		path = new List<int>();
		me = State.Searching;
		if (deadlyMouseButton == 1)
			playerTag = "PlayerBlack";
		else if (deadlyMouseButton == 0)
			playerTag = "PlayerWhite";
	}
	
	void Update() {
		if (me != State.Paralyzed) {
			Debug.DrawLine (transform.position,followMe.transform.position, Color.red);
			RaycastHit2D hit = Physics2D.Linecast (transform.position, followMe.transform.position, ~nodeAndEnemy);
			if ((hit.collider == null) || (hit.collider.name==followMe.name || hit.collider.tag == playerTag))
				me = State.HasSight;
			else
				me = State.Pathing;
		}
		switch (me) {
			case State.HasSight:
				if (path.Count > 0)
					path.Clear ();
				DumbSeek(followMe.transform.position);
				break;
			case State.Pathing:
				int youMoved = -1, start, end;
				Collider2D myCircle = Physics2D.OverlapCircle(transform.position, colliderSize, nodeMask);
				Collider2D targetCircle = Physics2D.OverlapCircle(followMe.transform.position, colliderSize, nodeMask);
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
				print("ERROR! SOMEHOW!\n");
				break;
		}
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
		Debug.Log ("AN ERROR HAS OCCURRED\n");
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
		int step = path[path.Count-1];
		////Vector3 dest = GenerateGraph.nodeVectors[step];
		Vector3 dest;
		GenerateGraph.nodes.TryGetValue (step, out dest);
		DumbSeek(dest);
		if (Vector3.Distance(transform.position, dest) < 0.1f)
			path.RemoveAt(path.Count-1);
	}
	
	void PrintPath(List<int> thispath) {
		string p = "";
		foreach (int n in thispath) {
			p = p + n + ", ";
		}
		Debug.Log (p.Substring (0,p.Length-2) + "\n");
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
	
	//Giving this it's own function to add room for death animations and sounds.
	void KillMe() {
		DestroyObject(gameObject);
	}
}