using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour {
	private LevelTransitionController main;
	public AudioClip click;
	private static bool loading, inTutorial;

	private AudioSource source;
	private float lowPitch = 0.9f, highPitch = 1.1f;

	void Start() {
		loading = false;
		inTutorial = false;
		main = Camera.main.GetComponent<LevelTransitionController>();
		source = Camera.main.GetComponent<AudioSource>();
	}

	void Update() {
		if (inTutorial && this.name == "Help") {
			Tutorial();
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
				print("Made by me\n");
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
}
