using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound", menuName = "Create New Sound")]
public class Sound : ScriptableObject
{
    public string SoundName;
    public AudioClip audioClip;
}
