using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public IEnumerator Cor_DoorOpen()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 3f, 0); // 3f 만큼 위로 이동 (원하는 만큼 조정)

        float duration = 2f; // 2초 동안 이동
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}
