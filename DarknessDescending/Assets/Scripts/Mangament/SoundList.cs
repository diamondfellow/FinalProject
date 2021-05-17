using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundList : MonoBehaviour
{
    public static SoundList soundList;
    public List<Sound> allGameSounds = new List<Sound>();
    public void Awake()
    {
        soundList = this;
    }
    public AudioClip GetAudioClip(string requestedSoundName)
    {
        foreach(Sound sound in allGameSounds)
        {
            if(sound.SoundName == requestedSoundName)
            {
                return sound.audioClip;
            }
        }
        Debug.LogError("No Sound id" + requestedSoundName);
        return null;
    }
}
