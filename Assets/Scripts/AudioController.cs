using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioController : MonoBehaviour {

	public enum SfxType {
		Button, RockSmash, PowerUpPickup, Bubble, Sprint, Flower, Wave, Voice, GameOver
	}

	[System.SerializableAttribute]
	public struct Sfx {
		public SfxType sfxType;
		public AudioClip[] clips;
 	}
    
    Dictionary<SfxType, AudioSource> sourceDictionary = new Dictionary<SfxType, AudioSource>();
    Dictionary<SfxType, AudioClip[]> clipsDictionary = new Dictionary<SfxType, AudioClip[]>();

    void Start()
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (clipsDictionary.ContainsKey(sfx[i].sfxType))
            {
                clipsDictionary[sfx[i].sfxType] = clipsDictionary[sfx[i].sfxType].Concat(sfx[i].clips).ToArray();
            }
            else
            {
                clipsDictionary.Add(sfx[i].sfxType, sfx[i].clips);
                sourceDictionary.Add(sfx[i].sfxType, gameObject.AddComponent<AudioSource>());
                sourceDictionary[sfx[i].sfxType].clip = sfx[i].clips[Random.Range(0, sfx[i].clips.Length)];
            }
        }
    }

	public Sfx[] sfx;
	public void Play(SfxType sfxType){
        sourceDictionary[sfxType].Play();
        AudioClip[] clips = clipsDictionary[sfxType];
        int index = Random.Range(0, clips.Length);
        sourceDictionary[sfxType].clip = clips[index];
    }
}
