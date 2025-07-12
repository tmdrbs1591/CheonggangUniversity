using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public static SongManager instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    private void Awake()
    {
        instance = this;
    }

    public void SongChange(int index)
    {
        if (index < 0 || index >= audioClips.Length)
        {
            Debug.LogWarning("ÀÎµ¦½º°¡ ¹üÀ§¸¦ ¹þ¾î³µ½À´Ï´Ù.");
            return;
        }

        audioSource.clip = audioClips[index];
        audioSource.Play();
    }
}
