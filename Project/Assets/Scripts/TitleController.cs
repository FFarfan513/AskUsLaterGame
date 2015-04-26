using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour {
	private LevelTransitionController main;
	public AudioClip click;
	private static bool loading;
	private bool inTutorial, inCredits;

	private AudioSource source;
	private float lowPitch = 0.9f, highPitch = 1.1f;

	void Start() {
		loading = false;
		main = Camera.main.GetComponent<LevelTransitionController>();
		source = Camera.main.GetComponent<AudioSource>();
	}

	void Update() {
		if (inTutorial && this.name == "Help") {
			Tutorial();
		}
		else if (inCredits && this.name == "Credits") {
			Credits();
		}
		if (Input.GetKey("escape")) {
			Application.Quit();
		}
	}

	void OnMouseDown() {
		if (!loading) {
			source.pitch = Random.Range (lowPitch, highPitch);
			source.PlayOneShot(click);
			if (this.name == "Play") {
				main.Next ();
				loading = true;
			}
			if (this.name == "Help") {
				inTutorial = true;
				Vector3 cam = new Vector3(-20,40,-10);
				Camera.main.transform.position = cam;
			}
			if (this.name == "Credits") {
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
