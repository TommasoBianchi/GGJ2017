using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public enum SfxType {
		Button, RockSmash, PowerUpPickup, Bubble, Sprint, Flower, Wave, Voice, GameOver
	}

	[System.SerializableAttribute]
	public struct Sfx {
		public SfxType sfxType;
		public AudioClip[] clips;
 	}

	public Sfx[] sfx;
	public void Play(SfxType sfxtype){
		int count, i;
		for (i=0 ; i<9 ; i++) {
			if (sfx[i].sfxType == sfxtype) {
				count = Random.Range(0 , sfx[i].clips.Length);
				gameObject.GetComponent<AudioSource>().clip = sfx[i].clips[count];
				gameObject.GetComponent<AudioSource>().Play();
			}
		}
	}
}
