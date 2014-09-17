using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioDatabase : MonoBehaviour {

    public List<AudioClip> soundEffects = new List<AudioClip>();
    public List<AudioClip> music = new List<AudioClip>();
	
    public AudioClip GetSoundEffect(string name)
    {
        for (int i = 0; i < soundEffects.Count; ++i)
        {
            if (soundEffects[i].name == name)
                return soundEffects[i];
        }
        return null;
    }

    public AudioClip GetMusic(string name)
    {
        for (int i = 0; i < music.Count; ++i)
        {
            if (music[i].name == name)
                return music[i];
        }
        return null;
    }
}
