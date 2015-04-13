using UnityEngine;
using System.Collections;

public class IdleController : MonoBehaviour
{
	private int idleBlackCount, idleWhiteCount;	// Count for idle for both players

	// Use this for initialization
	void Start () {
		idleBlackCount = 0;
		idleWhiteCount = 0;
	}
	
	// Update is called once per frame
	void Update () {

		// Finds the player game object to access its isMoving value
		GameObject playerBlack, playerWhite;
		playerBlack = GameObject.FindGameObjectWithTag ("PlayerBlack");
		playerWhite = GameObject.FindGameObjectWithTag ("PlayerWhite"); 

		// Update the idle count every update, and reset to 0 if it's moving
		idleWhiteCount++;
		idleBlackCount++;

		// Getting the MoveTo component to be able to access isMoving
		MoveTo black = playerBlack.GetComponent<MoveTo> ();
		MoveTo white = playerWhite.GetComponent<MoveTo> ();

		// Getting the Renderer object for each transparency square
		GameObject b = GameObject.Find("IdleBlack");
		SpriteRenderer idleB = b.GetComponent<SpriteRenderer> ();
		GameObject w = GameObject.Find("IdleWhite");
		SpriteRenderer idleW = w.GetComponent<SpriteRenderer> ();

		// Check to see if the player is moving, sets the idle count to 0 and the transparency to 0 if so
		if (black.isMoving) {
			idleBlackCount = 0;
			Color tempB = idleB.color;
			tempB.a = 0;
			idleB.color = tempB;
		}
		if (white.isMoving) {
			idleWhiteCount = 0;
			Color tempW = idleW.color;
			tempW.a = 0;
			idleW.color = tempW;
		}

		// If the idle count is over whatever value, then the renderer starts to become less transparent
		if (idleBlackCount > 180) {
			Color tempB = idleB.color;
			tempB.a += 0.005f;
			idleB.color = tempB;
		}

		if (idleWhiteCount > 180) {
			Color tempW = idleW.color;
			tempW.a += 0.005f;
			idleW.color = tempW;
		}

		//print (idleBlackCount);
		//print (idleWhiteCount);
	
	}
	
}

