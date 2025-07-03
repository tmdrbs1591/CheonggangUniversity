using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerParrying : MonoBehaviour
{
    [SerializeField] private GameObject parryingObject;
    [SerializeField] private GameObject parryingVolume;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public bool isParrying;
    private PlayerBase player;

    private float defaultOrthoSize;
    private Quaternion defaultRotation;

    private void Awake()
    {
        player = GetComponent<PlayerBase>();
    }

    private void Start()
    {
        parryingObject.SetActive(false);

        if (virtualCamera != null)
        {
            // Orthographic 줌 크기와 회전값 저장
            defaultOrthoSize = virtualCamera.m_Lens.OrthographicSize;
            defaultRotation = virtualCamera.transform.rotation;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(Cor_ParryingObject());
        }
    }

    public IEnumerator Cor_ParryingObject()
    {
        player.AnimationTrigger("Shot");
        isParrying = true;
        parryingObject.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        isParrying = false;
        parryingObject.SetActive(false);
    }

    /// <summary>
    /// 패링 성공 시 호출되는 함수
    /// </summary>
    public void ParryingStart()
    {
        GameManager.instance.Flash();
        CameraShake.instance.ShakeCamera(10f, 0.15f);
        StartCoroutine(Cor_TimeSlow());
        ObjectPool.SpawnFromPool("ParryingEffect", parryingObject.transform.position);
        AudioManager.instance?.PlaySound(transform.position, "Parrying", UnityEngine.Random.Range(0.9f, 1f), 1f);
        AudioManager.instance?.PlaySound(transform.position, "Parrying2", UnityEngine.Random.Range(1f, 1.1f), 1f);
        AudioManager.instance?.PlaySound(transform.position, "Parrying3", UnityEngine.Random.Range(1f, 1f), 1f);
        if (virtualCamera != null)
            StartCoroutine(Cor_ParryingCameraEffect());
        else
            Debug.LogWarning("virtualCamera가 비어있음!");
    }

    /// <summary>
    /// 카메라 줌 + 회전 효과 코루틴
    /// </summary>
    private IEnumerator Cor_ParryingCameraEffect()
    {
        float zoomSize = defaultOrthoSize * 0.7f; // 줌인 비율
        Quaternion tiltRotation = Quaternion.Euler(10f, 0f, 0f); // 위로 10도
        float duration = 0.15f;
        float t = 0f;

        // 줌 + 회전 적용
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(defaultOrthoSize, zoomSize, lerp);
            virtualCamera.transform.rotation = Quaternion.Slerp(defaultRotation, tiltRotation, lerp);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f); // 잠깐 유지

        // 줌 + 회전 원상 복구
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(zoomSize, defaultOrthoSize, lerp);
            virtualCamera.transform.rotation = Quaternion.Slerp(tiltRotation, defaultRotation, lerp);

            yield return null;
        }
    }
    private IEnumerator Cor_TimeSlow()
    {
        parryingVolume.SetActive(true);
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        parryingVolume.SetActive(false);

    }
}
