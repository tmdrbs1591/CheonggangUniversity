using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public PlayerBase playerbase;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject skillLaserPrefab;

    [SerializeField] GameObject laserRotation;
    [SerializeField] GameObject laserRotationTrail;

    [SerializeField] public Slider bigLaserValueSlider;
    [SerializeField] private float maxBigLaserValue;
    [SerializeField] private float currentBigLaserValue;

    [Header("AttackArea Settings")]
    [SerializeField] public Transform windAttackPos;
    [SerializeField] public Vector2 windAttackBoxSize;
    [SerializeField] public Transform attackPos;
    [SerializeField] public Vector2 attackBoxSize;


    [SerializeField] GameObject attackSlash1;
    [SerializeField] GameObject attackSlash2;
    private bool useFirstSlash = true;
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

        // 회전 적용 (음수든 양수든 상관없이 유지)
        laserRotation.transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 scale = laserRotation.transform.localScale;
        float sign = playerbase.transform.localScale.x < 0 ? -1f : 1f;
        scale.x = Mathf.Abs(scale.x) * sign;
        laserRotation.transform.localScale = scale;

        // 기존 처리
        if (playerbase.currentAttackType == AttackType.Gun)
            UpdateLaserTrail(direction);
        else
            DisableLaserTrail();
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
    private void DisableLaserTrail()
    {
        Vector3 startPoint = firePoint.position;
        LineRenderer lr = laserRotationTrail.GetComponent<LineRenderer>();
        lr.SetPosition(0, startPoint);
        lr.SetPosition(1, startPoint); // 길이 0
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
                damageable.TakeDamage(Random.Range(playerbase.playerStat.attackPower, playerbase.playerStat.attackPower + 10));
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

    public void SwordSlashToggle()
    {
        if (useFirstSlash)
        {
            attackSlash1.SetActive(true);
            attackSlash2.SetActive(false);
        }
        else
        {
            attackSlash1.SetActive(false);
            attackSlash2.SetActive(true);
        }

        useFirstSlash = !useFirstSlash;
    }
    public IEnumerator SwordSkill()
    {
        if (currentBigLaserValue >= maxBigLaserValue)
        {
            for (int i = 0; i < 5; i++)
            {
                AudioManager.instance?.PlaySound(transform.position, "Sword", Random.Range(1f, 1.2f), 1f);
                Damage(windAttackPos, windAttackBoxSize);
                yield return new WaitForSeconds(0.08f);

                SwordSlashToggle();
            }
            currentBigLaserValue = 0;
            bigLaserValueSlider.value = currentBigLaserValue / maxBigLaserValue;
        }
    }
    public void Damage(Transform t, Vector2 v)
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(t.position, v, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log("공격!");
            if (collider != null)
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    currentBigLaserValue++;
                    damageable.TakeDamage(Random.Range(playerbase.playerStat.attackPower, playerbase.playerStat.attackPower + 5));
                    bigLaserValueSlider.value = currentBigLaserValue / maxBigLaserValue;

                }

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(windAttackPos.position, windAttackBoxSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPos.position, attackBoxSize);
    }
}
