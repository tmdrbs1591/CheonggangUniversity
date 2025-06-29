using System.Collections;
using UnityEngine;

public class StoneButton : MonoBehaviour, Interaction
{
    [SerializeField] Transform stone;
    [SerializeField] Transform stoneMovePos;
    [SerializeField] float moveTime = 1.5f;

    bool isOpen = false;
    public void EventStart()
    {
        if (isOpen)
            return;
        StartCoroutine(MoveStoneCoroutine());
        TimeLineManager.instance.StartCutScene(1);
        isOpen = true;
    }

    private IEnumerator MoveStoneCoroutine()
    {
        yield return new WaitForSeconds(3f);
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

        // ������ ��ġ ��Ȯ�� ����
        stone.position = endPos;
    }
}
