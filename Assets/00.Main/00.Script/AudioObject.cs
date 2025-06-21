// AudioObject.cs
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioObject : MonoBehaviour
{
    private AudioSource audioSource;

    [HideInInspector] public AudioClip clip;
    [HideInInspector] public float pitch = 1f;
    [HideInInspector] public float volume = 1f;
    [HideInInspector] public Transform follow;

    private bool isFollowing;
    private const float zOffset = -5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume; 
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; 
        audioSource.Play();

        isFollowing = follow != null;
    }

    void Update()
    {
        if (isFollowing)
        {
            if (follow != null)
                transform.position = new Vector3(follow.position.x, follow.position.y, zOffset);
            else
                Destroy(gameObject);
        }

        if (!audioSource.isPlaying)
            Destroy(gameObject); // 재생 끝나면 삭제
    }
}
