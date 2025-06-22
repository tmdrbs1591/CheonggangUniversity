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

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D ����
    }

    // ������Ʈ Ǯ���� ���� �� �ʱ�ȭ�� �޼���
    public void Init(AudioClip clip, float pitch, float volume, Transform follower)
    {
        this.clip = clip;
        this.pitch = pitch;
        this.volume = volume;
        this.follow = follower;

        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();

        isFollowing = follower != null;
    }

    void Update()
    {
        if (isFollowing)
        {
            if (follow != null)
                transform.position = new Vector3(follow.position.x, follow.position.y, zOffset);
            else
                ObjectPool.ReturnToPool("AudioObject", gameObject);
        }

        // ����� ������ Ǯ�� ��ȯ
        if (!audioSource.isPlaying)
            ObjectPool.ReturnToPool("AudioObject", gameObject);
    }
}
