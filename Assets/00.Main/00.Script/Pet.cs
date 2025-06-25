using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField] private Transform player; // 따라갈 플레이어
    [SerializeField] private float followDistance = 1.5f; // 플레이어로부터 떨어질 거리
    [SerializeField] private float followSpeed = 5f; // 따라가는 속도
    [SerializeField] private float heightOffset = 0.5f; // 펫의 높이 보정
    [SerializeField] private float smoothTime = 0.1f; // 움직임 부드럽게

    private Vector3 velocity = Vector3.zero;
    private float facingDir = 1; // 플레이어가 바라보는 방향 (1: 오른쪽, -1: 왼쪽)

    private void Update()
    {
        if (player == null || TimeLineManager.instance.isCutScene) return;

        // 플레이어가 바라보는 방향을 감지 (scale.x 기준으로 추정)
        facingDir = player.localScale.x > 0 ? 3.6f : -3.6f;

        // 목표 위치 계산: 플레이어의 반대 방향에 따라가게 함
        Vector3 targetPos = player.position + new Vector3(-facingDir * followDistance, heightOffset, 0);

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // 펫이 플레이어 바라보는 방향을 향하도록 (회전)
        Vector3 localScale = transform.localScale;
        localScale.x = facingDir;
        transform.localScale = localScale;
    }
}
