using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject skillLaserPrefab;

    [SerializeField] GameObject laserRotation;
    [SerializeField] GameObject laserRotationTrail;

    [SerializeField] Slider bigLaserValueSlider;
    [SerializeField] private float maxBigLaserValue;
    [SerializeField] private float currentBigLaserValue;

    private void Start()
    {
        currentBigLaserValue = maxBigLaserValue;
        bigLaserValueSlider.value = currentBigLaserValue / maxBigLaserValue;
    }
    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.transform.position.z * -1f;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laserRotation.transform.rotation = Quaternion.Euler(0, 0, angle);

        UpdateLaserTrail(direction);
    }

    private void UpdateLaserTrail(Vector2 direction)
    {
        float maxDistance = 30f;
        Vector3 startPoint = firePoint.position;
        Vector3 endPoint = startPoint + (Vector3)(direction * maxDistance);

        RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, direction, maxDistance);

        float closestDistance = maxDistance;

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float dist = Vector3.Distance(startPoint, hit.point);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    endPoint = hit.point;
                }
            }
        }

        LineRenderer lr = laserRotationTrail.GetComponent<LineRenderer>();
        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, endPoint);
    }

    public void Fire(Vector2 direction)
    {
        CameraShake.instance.ShakeCamera(2f, 0.2f);
        AudioManager.instance?.PlaySound(transform.position, "레이저", Random.Range(1f, 1.2f), 1f);
        AudioManager.instance?.PlaySound(transform.position, "총", Random.Range(1f, 1.2f), 1f);

        float maxDistance = 15f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, direction, maxDistance);

        Vector3 endPoint = firePoint.position + (Vector3)(direction * maxDistance);
        float closestDistance = maxDistance;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            float dist = Vector3.Distance(firePoint.position, hit.point);
            if (dist >= closestDistance) continue;

            // 벽인 경우: 레이저 끝점만 조정
            if (hit.collider.CompareTag("Wall"))
            {
                closestDistance = dist;
                endPoint = hit.point;
                ObjectPool.SpawnFromPool("BulletEffect", hit.point);
                ObjectPool.SpawnFromPool("HitEffect", hit.point);
                continue;
            }

            // IDamageable을 가진 오브젝트 처리
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            EnemyBase enemyBase = hit.collider.GetComponent<EnemyBase>();

            if (damageable != null && (enemyBase == null || (enemyBase != null && !enemyBase.isDying)))
            {
                closestDistance = dist;
                endPoint = hit.point;

                ObjectPool.SpawnFromPool("BulletEffect", hit.point);
                ObjectPool.SpawnFromPool("HitEffect", hit.point);
                damageable.TakeDamage(Random.Range(10,20));
                currentBigLaserValue++; // 빅 레이저 벨류 증가
                bigLaserValueSlider.value = currentBigLaserValue / maxBigLaserValue;

            }
        }

        GameObject go = ObjectPool.SpawnFromPool("BulletLaser", transform.position);
        LineRenderer lr = go.GetComponent<LineRenderer>();

        // 길이에 비례한 지속 시간 계산
        float laserLength = Vector3.Distance(firePoint.position, endPoint);
        float speed = 100f; // 레이저 진행 속도 (너가 원하는 값으로 조정 가능)
        float duration = laserLength / speed;

        StartCoroutine(LaserEffect(lr, firePoint.position, endPoint, duration));
    }


    private IEnumerator LaserEffect(LineRenderer lr, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 currentStart = Vector3.Lerp(start, end, t);

            lr.SetPosition(0, currentStart);
            lr.SetPosition(1, end);

            yield return null;
        }

        ObjectPool.ReturnToPool("BulletLaser", lr.gameObject);
    }

    public void SkillLaserFire()
    {
        if(currentBigLaserValue >= maxBigLaserValue)
        {
            AudioManager.instance?.PlaySound(transform.position, "서브레이저", Random.Range(1f, 1.1f), 1f);
            AudioManager.instance?.PlaySound(transform.position, "서브레이저2", Random.Range(1f, 1.1f), 1f);
            CameraShake.instance.ShakeCamera(8f, 0.3f);
            skillLaserPrefab.SetActive(false);
            skillLaserPrefab.SetActive(true);
            currentBigLaserValue = 0;
            bigLaserValueSlider.value = currentBigLaserValue / maxBigLaserValue;
        }
    }
}
