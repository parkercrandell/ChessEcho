using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip winClip;
    public AudioClip moveClip;
    public AudioClip killClip;

    public void PlayMove()
    {
        audioSource.PlayOneShot(moveClip);
    }

    public void PlayKill()
    {
        audioSource.PlayOneShot(killClip);
    }

    public void PlayWin()
    {
        audioSource.PlayOneShot(winClip);
    }
}
