using System.Collections;
using UnityEngine;

public class ElectricChain : MonoBehaviour
{
    public Transform[] electricNodes; // 순서대로 연결될 노드
    public LineRenderer lineRenderer;
    public float drawDurationPerSegment = 0.2f;

    bool isActive;
    public Door door;

    void Start()
    {
        lineRenderer.positionCount = 0;
    }

    public void ActivateElectricChain()
    {
        if (isActive)
            return;
        StartCoroutine(DrawElectricLineSmoothly());
        isActive = true;
    }

    IEnumerator DrawElectricLineSmoothly()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, electricNodes[0].position);

        for (int i = 1; i < electricNodes.Length; i++)
        {
            Vector3 startPos = electricNodes[i - 1].position;
            Vector3 endPos = electricNodes[i].position;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / drawDurationPerSegment;
                Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);

                lineRenderer.positionCount = i + 1;
                lineRenderer.SetPosition(i, currentPos); // 점점 늘어남
                CameraShake.instance.ShakeCamera(5f, 1f);
                yield return null;
            }
            AudioManager.instance?.PlaySound(transform.position, "Electric", UnityEngine.Random.Range(1f, 1.2f), 1f);

            // 최종 위치 정확하게 고정
            lineRenderer.SetPosition(i, endPos);
        }
        StartCoroutine(door.Cor_DoorOpen());
    }
}
