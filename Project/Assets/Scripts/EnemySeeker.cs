using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemySeeker : MonoBehaviour {
	EnemyController controller;

	private List<int> path;
	private Vector3 followPos;

	void Start() {
		controller = this.GetComponent<EnemyController>();
		path = new List<int>();
	}
	
	void Update() {
		//If this enemy is not paralyzed, check for sight.
		if (controller.GetState() != EnemyController.State.Paralyzed) {
			if (controller.CanSeeIt(transform.position))
				controller.SetState(EnemyController.State.HasSight);
			else
				controller.SetState(EnemyController.State.NoSight);
		}
		followPos = controller.followMe.transform.position;

		switch (controller.GetState())
		{
		case EnemyController.State.HasSight:
			//If it has sight, dumb seek towards the player.
			if (path.Count > 0)
				path.Clear ();
			transform.position = controller.DumbSeek(transform.position, followPos);
			break;
		case EnemyController.State.NoSight:
			//If it doesn't have sight, start an A* search from the closest node to
			//this enemy's position, and followMe's position
			int youMoved = -1, start, end;
			Collider2D myNode = controller.FindNodeNearXClosestToY(transform.position, followPos);
			Collider2D targetNode = controller.FindNodeNearXClosestToY(followPos, followPos);
			if (myNode!=null && targetNode!= null) {
				//if there is no path yet
				if (path.Count == 0) {
					if (int.TryParse(myNode.name, out start) && int.TryParse(targetNode.name, out end)) {
						path = controller.Astar(start, end);
					}
				}
				//else if closest node to player has changed
				else if (int.TryParse(targetNode.name, out youMoved) && (youMoved != path[0]) && !path.Contains (youMoved)) {
					path = controller.Astar(path[path.Count-1], youMoved);
				}
				MoveThroughPath();
			}
			break;
		case EnemyController.State.Paralyzed:
			break;
		default:
			print("ERROR! The state was somehow set to something other than it's enum values.\n");
			break;
		}
	}
	

	void MoveThroughPath() {
		//this function always moves to the last node in the path, and removes it when it reaches it.
		if (path.Count > 0) {
			int step = path[path.Count-1];
			Vector3 dest;
			if (GenerateGraph.nodes.TryGetValue (step, out dest)) {
				transform.position = controller.DumbSeek(transform.position, dest);
				if (Vector3.Distance(transform.position, dest) < 0.1f)
					path.RemoveAt(path.Count-1);
			}
		}
		
		else if (path.Count == 0) {
			controller.SetState(EnemyController.State.HasSight);
		}
	}

	void OnTriggerEnter2D( Collider2D other ) {
		if (controller != null && other.tag == controller.GetPlayerTag()) {
			controller.Damage();
			controller.KillMe();
		}
	}
}
