using System.Collections;
using UnityEngine;

public class LevelUpItemChest : MonoBehaviour
{
    [SerializeField] private GameObject chestItemPrefabs; // UI 프리팹 (캔버스 자식)
    [SerializeField] private RectTransform[] itemPos;     // 도착 위치들 (UI 기준)
    [SerializeField] private RectTransform spawnOrigin;   // 시작 위치 (이 오브젝트 기준)
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private float curveHeight = 100f;    // UI는 단위가 작으니 100 정도 추천

    void Start()
    {
        StartCoroutine(ItemSpawn());
    }

    IEnumerator ItemSpawn()
    {
        float delayBetween = 0.1f; // 0.1초 텀

        foreach (RectTransform target in itemPos)
        {
            GameObject item = Instantiate(chestItemPrefabs, spawnOrigin.position, Quaternion.identity, spawnOrigin.parent);
            RectTransform itemRect = item.GetComponent<RectTransform>();

            Vector2 start = spawnOrigin.anchoredPosition;
            Vector2 end = target.anchoredPosition;
            Vector2 control = (start + end) / 2f + Vector2.up * curveHeight;

            StartCoroutine(MoveAlongCurve(itemRect, start, control, end));

            yield return new WaitForSeconds(delayBetween); // 0.1초 텀
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
