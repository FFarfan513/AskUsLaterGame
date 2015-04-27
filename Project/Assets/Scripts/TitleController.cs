using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour {
	private LevelTransitionController main;
	public AudioClip click;
	private static bool loading;
	private bool inTutorial, inCredits;

	private AudioSource source;
	private float lowPitch = 0.9f, highPitch = 1.1f;

	private bool playButton = false, helpButton = false, creditsButton = false;

	void Start() {
		if (this.name == "Play") {
			playButton = true;
		}
		else if (this.name == "Help") {
			helpButton = true;
		}
		else if (this.name == "Credits") {
			creditsButton = true;
		}

		loading = false;
		main = Camera.main.GetComponent<LevelTransitionController>();
		source = Camera.main.GetComponent<AudioSource>();
	}

	//Pressing the Help button 
	void Update() {
		if (inTutorial && helpButton) {
			Tutorial();
		}
		else if (inCredits && creditsButton) {
			Credits();
		}

		//quits the game, if in the main menu
		if (Input.GetKey("escape")) {
			Application.Quit();
		}
	}

	void OnMouseDown() {
		if (!loading) {
			source.pitch = Random.Range (lowPitch, highPitch);
			source.PlayOneShot(click);
			if (playButton) {
				main.Next();
				loading = true;
			}
			else if (helpButton) {
				inTutorial = true;
				Vector3 cam = new Vector3(-20,40,-10);
				Camera.main.transform.position = cam;
			}
			else if (creditsButton) {
				inCredits = true;
				Camera.main.transform.position = new Vector3(-20,-40,-10);
			}
		}
	}

	void Tutorial() {
		Vector3 camTemp = Camera.main.transform.position;

		if(Input.GetMouseButtonDown (0))
			camTemp.x += 20;
		if(Input.GetMouseButtonDown (1))
			camTemp.x -= 20;
		
		if(camTemp.x > 101 || camTemp.x < -1) {
			camTemp.x = 0;
			camTemp.y = 0;
			inTutorial = false;
		}
		
		Camera.main.transform.position = camTemp;
	}
	
	void Credits() {
		Vector3 camTemp = Camera.main.transform.position;

		if(Input.GetMouseButtonDown (0) || Input.GetMouseButtonDown(1) )
			camTemp.x += 20;
		
		if(camTemp.x > 1) {
			camTemp.x = 0;
			camTemp.y = 0;
			inCredits = false;
		}
		
		Camera.main.transform.position = camTemp;
	}
}
