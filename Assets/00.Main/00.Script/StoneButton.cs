using System.Collections;
using UnityEngine;

public class StoneButton : MonoBehaviour, Interaction
{
    [SerializeField] Transform stone;
    [SerializeField] Transform stoneMovePos;
    [SerializeField] float moveTime = 1.5f;

    public void EventStart()
    {
        StartCoroutine(MoveStoneCoroutine());
        TimeLineManager.instance.StartCutScene(1);
    }

    private IEnumerator MoveStoneCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Vector3 startPos = stone.position;
        Vector3 endPos = stoneMovePos.position;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;
            stone.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // 마지막 위치 정확히 고정
        stone.position = endPos;
    }
}
