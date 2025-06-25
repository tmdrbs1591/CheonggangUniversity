using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField] private Transform player; // ���� �÷��̾�
    [SerializeField] private float followDistance = 1.5f; // �÷��̾�κ��� ������ �Ÿ�
    [SerializeField] private float followSpeed = 5f; // ���󰡴� �ӵ�
    [SerializeField] private float heightOffset = 0.5f; // ���� ���� ����
    [SerializeField] private float smoothTime = 0.1f; // ������ �ε巴��

    private Vector3 velocity = Vector3.zero;
    private float facingDir = 1; // �÷��̾ �ٶ󺸴� ���� (1: ������, -1: ����)

    private void Update()
    {
        if (player == null || TimeLineManager.instance.isCutScene) return;

        // �÷��̾ �ٶ󺸴� ������ ���� (scale.x �������� ����)
        facingDir = player.localScale.x > 0 ? 3.6f : -3.6f;

        // ��ǥ ��ġ ���: �÷��̾��� �ݴ� ���⿡ ���󰡰� ��
        Vector3 targetPos = player.position + new Vector3(-facingDir * followDistance, heightOffset, 0);

        // �ε巴�� �̵�
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // ���� �÷��̾� �ٶ󺸴� ������ ���ϵ��� (ȸ��)
        Vector3 localScale = transform.localScale;
        localScale.x = facingDir;
        transform.localScale = localScale;
    }
}
