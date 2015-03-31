using UnityEngine;
using System.Collections;

public class PlayMusic : MonoBehaviour {
	
	//set these two in Unity:
	public AudioSource intro;
	public AudioSource loop;
	
	private bool looping = false;
	private int length;
	private int offset = 1000;
	
	void Awake() {
		if (loop.playOnAwake) {
			loop.playOnAwake = false;
			loop.Stop ();
		}

		length = intro.clip.samples;
		if (!intro.playOnAwake)
			intro.Play();
	}
	
	void FixedUpdate () {
		if (!looping && (intro.timeSamples >= length-offset)) {
			loop.Play ();
			looping = true;
		}
	}
}
