using System.Collections;
using UnityEngine;

public class LevelUpItemChest : MonoBehaviour
{
    [SerializeField] private GameObject chestItemPrefabs; // UI ������ (ĵ���� �ڽ�)
    [SerializeField] private RectTransform[] itemPos;     // ���� ��ġ�� (UI ����)
    [SerializeField] private RectTransform spawnOrigin;   // ���� ��ġ (�� ������Ʈ ����)
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private float curveHeight = 100f;    // UI�� ������ ������ 100 ���� ��õ

    void Start()
    {
        StartCoroutine(ItemSpawn());
    }

    IEnumerator ItemSpawn()
    {
        float delayBetween = 0.1f; // 0.1�� ��

        foreach (RectTransform target in itemPos)
        {
            GameObject item = Instantiate(chestItemPrefabs, spawnOrigin.position, Quaternion.identity, spawnOrigin.parent);
            RectTransform itemRect = item.GetComponent<RectTransform>();

            Vector2 start = spawnOrigin.anchoredPosition;
            Vector2 end = target.anchoredPosition;
            Vector2 control = (start + end) / 2f + Vector2.up * curveHeight;

            StartCoroutine(MoveAlongCurve(itemRect, start, control, end));

            yield return new WaitForSeconds(delayBetween); // 0.1�� ��
        }
    }

    IEnumerator MoveAlongCurve(RectTransform obj, Vector2 start, Vector2 control, Vector2 end)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;

            Vector2 m1 = Vector2.Lerp(start, control, t);
            Vector2 m2 = Vector2.Lerp(control, end, t);
            obj.anchoredPosition = Vector2.Lerp(m1, m2, t);

            yield return null;
        }

        obj.anchoredPosition = end;
    }
}
