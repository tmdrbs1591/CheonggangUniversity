using UnityEngine;

public class EXP : MonoBehaviour
{
    private Vector2 startPosition;
    private Vector2 controlPoint;
    private Vector2 targetPosition;

    private float moveTime = 0f;
    [SerializeField] private float duration = 1.5f;

    private bool isMoving = false;

    private Transform playerTransform;

    private Vector2 randomDirection;

    private void OnEnable()
    {
        moveTime = 0f;
        isMoving = true;

        playerTransform = GameManager.instance.playerCont.transform;
        startPosition = transform.position;

        float randomAngle = Random.Range(0f, 360f);
        randomDirection = new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));

        UpdateTargetAndControlPoint();
    }

    void Update()
    {
        if (!isMoving) return;

        UpdateTargetAndControlPoint();

        moveTime += Time.deltaTime / duration;
        if (moveTime > 1f) moveTime = 1f;

        Vector2 p0 = startPosition;
        Vector2 p1 = controlPoint;
        Vector2 p2 = targetPosition;

        Vector2 newPos = Mathf.Pow(1 - moveTime, 2) * p0 +
                         2 * (1 - moveTime) * moveTime * p1 +
                         Mathf.Pow(moveTime, 2) * p2;

        transform.position = newPos;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < 0.3f || moveTime >= 1f)
        {
            ObjectPool.ReturnToPool("EXP", gameObject);
        }
    }

    private void UpdateTargetAndControlPoint()
    {
        targetPosition = playerTransform.position;
        Vector2 midpoint = (startPosition + targetPosition) * 0.5f;

        Vector2 randomOffset = randomDirection * 4f;

        controlPoint = midpoint + randomOffset;
    }
}
