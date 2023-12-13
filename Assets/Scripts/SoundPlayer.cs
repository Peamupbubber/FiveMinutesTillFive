using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] clips;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void PlayRandomSound()
    {
        audioSource.PlayOneShot(clips[Random.Range(0,clips.Length-1)]);
    }
    void PlaySound(int index)
    {
        audioSource.PlayOneShot(clips[index]);
    }
}
