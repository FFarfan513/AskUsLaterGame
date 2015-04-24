using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour {
	private LevelTransitionController main;
	public AudioClip click;
	private static bool loading;

	private AudioSource source;
	private float lowPitch = 0.9f, highPitch = 1.1f;

	void Start() {
		loading = false;
		main = Camera.main.GetComponent<LevelTransitionController>();
		source = Camera.main.GetComponent<AudioSource>();
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
				print("Help!\n");
			}
			if (this.name == "Credits") {
				print("Made by me\n");
			}
		}
	}
}
