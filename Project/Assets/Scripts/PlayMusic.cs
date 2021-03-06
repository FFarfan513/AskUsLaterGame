﻿using UnityEngine;
using System.Collections;

public class PlayMusic : MonoBehaviour {
	
	//set these two in Unity:
	public AudioSource intro;
	public AudioSource loop;

	//noIntro allows us to have a song that loops perfectly and avoids playing the intro first
	private bool noIntro;
	private bool looping = false;
	private int length;
	private int offset = 1000;
	
	void Awake() {
		if (intro == null)
			noIntro = true;

		if (!noIntro) {
			//if loop has it's playOnAwake on, set it off and stop it.
			if (loop.playOnAwake) {
				loop.playOnAwake = false;
				loop.Stop();
			}
			//Play the intro if it's not playing by itself
			length = intro.clip.samples;
			if (!intro.playOnAwake)
				intro.Play();
		}
		else {
			//if there's no intro, play loop.
			if (!loop.playOnAwake) {
				loop.Play();
				looping = true;
			}
		}
	}
	
	void Update () {
		//if the intro is playing and it's getting near the end, play loop.
		if (!looping && !noIntro && (intro.timeSamples >= length-offset)) {
			loop.Play();
			looping = true;
		}
	}
}
