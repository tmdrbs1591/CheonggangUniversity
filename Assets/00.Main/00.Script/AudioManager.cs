// AudioManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SoundData
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("사운드 목록")]
    public List<SoundData> soundDataList;
    public GameObject audioObjectPrefab;

    private Dictionary<string, AudioClip> soundDict;

    [Header("전체 볼륨 슬라이더")]
    public Slider volumeSlider;

    private float masterVolume = 1f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        soundDict = new Dictionary<string, AudioClip>();
        foreach (var data in soundDataList)
        {
            if (!soundDict.ContainsKey(data.name))
                soundDict.Add(data.name, data.clip);
            else
                Debug.LogWarning($"중복된 사운드 이름: {data.name}");
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = masterVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private void OnVolumeChanged(float value)
    {
        masterVolume = value;
        // 필요하면 여기서 BGM 등도 조절 가능
    }

    public void PlaySound(Vector3 position, string soundName, float pitch = 1f, float volume = 1f, Transform follower = null)
    {
        if (!soundDict.TryGetValue(soundName, out var clip))
        {
            Debug.LogWarning($"사운드 '{soundName}' 없음!");
            return;
        }

        GameObject obj = Instantiate(audioObjectPrefab, new Vector3(position.x, position.y, -5f), Quaternion.identity);
        AudioObject audObj = obj.GetComponent<AudioObject>();
        audObj.follow = follower;
        audObj.clip = clip;
        audObj.pitch = pitch;
        audObj.volume = volume * masterVolume;  // AudioManager 볼륨 곱해서 전달
    }
}
