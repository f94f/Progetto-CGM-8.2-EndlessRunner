using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManageScript : MonoBehaviour
{
    private AudioSource audioSource;
    public static AudioManageScript current; //per interfecciare con altri script, in moda da richiamarla

    private void Awake() //la void viene letta quando il gioco parte
    {
        current = this; //è un istanza di questo script, serve per interfacciare altri script con questo
    }

    
    public void PlaySound(AudioClip clip)
    {
        audioSource= GetComponent<AudioSource>();
        audioSource.clip = clip;

        if (clip.name == "diamondFx")
        {
            audioSource.volume = 0.3f;
        }
        else
        {
            audioSource.volume = 0.5f;
        }
        audioSource.Play();
    }

    
}
