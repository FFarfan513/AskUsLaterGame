using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GenerateGraph : MonoBehaviour {
	public float[] leftRightUpDown1 = new float[4];
	public float[] leftRightUpDown2 = new float[4];
	public float spacing; //keeping this at 3 is okay I think.
	private float radius = 0.9f;
	public GameObject nodePrefab;

	public static Dictionary <int,Vector3> nodes;
	private Dictionary <Vector3,int> sedon;
	
	public static Dictionary <int, List<int> > graph;
	private List<int> connections;
	private int ID = 0;
	
	private bool showMe;
	public static bool graphLoaded;
	private LayerMask noNodes;
	
	void Awake () {
		noNodes = ~LayerMask.GetMask ("Node");
		if (!graphLoaded) {
			nodes = new Dictionary<int,Vector3>();
			CreateNodes(leftRightUpDown1);
			CreateNodes(leftRightUpDown2);
			ConnectGraph();
			//PrintGraph ();
		}
	}
	
	void CreateNodes(float[] leftRightUpDown) {
		//Be careful of float rounding error!
		float xcurr = leftRightUpDown[0];
		float ycurr = leftRightUpDown[2];
		while (ycurr >= leftRightUpDown[3]) {
			Vector3 nodePos = new Vector3(xcurr,ycurr,0);
			//make sure there isn't an object with a collider (a wall) where we are creating this node
			bool occupied = false;
			Collider2D[] check = Physics2D.OverlapCircleAll(nodePos, radius);
			foreach (var col in check) {
				if (col.tag == "Wall") {
					occupied = true;
					break;
				}
			}
			//create a new node and add it to nodes with it's position
			if (!occupied) {
				GameObject node = Instantiate(nodePrefab, nodePos, Quaternion.identity) as GameObject;
				nodes.Add(ID,nodePos);
				node.name = "" + ID++;
			}
			xcurr += spacing;
			//if xcurr is past the right edge, move down one in the y, and set xcurr back to the left
			if (xcurr > leftRightUpDown[1]) {
				xcurr = leftRightUpDown[0];
				ycurr -= spacing;
			}
		}
	}
	
	void ConnectGraph() {
		sedon = new Dictionary<Vector3,int>(nodes.Count);
		foreach (var node in nodes) {
			sedon.Add (node.Value,node.Key);
		}
		graph = new Dictionary<int, List<int> >(nodes.Count);
		Vector3 left  = new Vector3 (-spacing, 0, 0);
		Vector3 right = new Vector3 (spacing,  0, 0);
		Vector3 up    = new Vector3 (0, spacing,  0);
		Vector3 down  = new Vector3 (0, -spacing, 0);

		foreach (var node in nodes) {
			Vector3 pos = node.Value;
			connections = new List<int>(8);
			
			//Look at all 8 standard directions, to see if there's a node there.
			//If so, add that node to this node's adjacency list.
			Lookup(pos, pos+left+up);
			Lookup(pos, pos+up);
			Lookup(pos, pos+right+up);
			Lookup(pos, pos+left);
			Lookup(pos, pos+right);
			Lookup(pos, pos+left+down);
			Lookup(pos, pos+down);
			Lookup(pos, pos+right+down);
			if (connections.Count == 0)
				Debug.Log("Warning: node " + node.Key + " is not connected to any other node in this graph\n");
			graph.Add(node.Key,connections);
		}
		graphLoaded = true;
	}
	
	void Lookup(Vector3 pos, Vector3 test) {
		//checks if there is an object in the way of a connection, and if there is, don't put a connection there.
		Vector3 dist = test - pos;
		RaycastHit2D obstruct = Physics2D.CircleCast(pos, 1f, dist, dist.magnitude, noNodes);

		if (obstruct.collider==null || obstruct.collider.tag != "Wall") {
			int nodeNum;
			if (sedon.TryGetValue (test, out nodeNum))
				connections.Add (nodeNum);
		}
	}

	void OnDrawGizmos() {
		//This draws the paths between the nodes.
		if (graphLoaded && showMe) {
			Gizmos.color = Color.magenta;
			foreach (var n in graph) {
				Vector3 node;
				nodes.TryGetValue (n.Key, out node);
				foreach (int adj in n.Value) {
					Vector3 neigh;
					nodes.TryGetValue (adj, out neigh);
					Gizmos.DrawLine (node, neigh);
				}
			}
		}
	}
	
	void Update() {
		if (Input.GetKeyDown (KeyCode.Z))
			showMe = !showMe;
		if (Input.GetKey("escape"))
			Application.Quit();
	}
	
	void PrintGraph() {
		foreach (var el in graph) {
			string s = "[";
			foreach (int adj in el.Value) {
				if (s.Length > 1)
					s = s + ", " + adj;
				else
					s = s + adj;
			}
			s = s + "]";
			Debug.Log (el.Key + " connected to: " + s + "\n");
		}
	}
}
