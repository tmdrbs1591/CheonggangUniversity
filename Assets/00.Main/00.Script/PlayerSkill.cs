using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerSkill : MonoBehaviour
{
    [Header("쿨타임 설정")]
    [SerializeField] private float qCooldown = 2f;
    [SerializeField] private float wCooldown = 5f;
    [SerializeField] private float eCooldown = 3f;
    [SerializeField] private float rCooldown = 10f;

    [Header("Q 스킬 UI")]
    [SerializeField] private Slider qSlider;
    [SerializeField] private TMP_Text qCooldownText;
    [SerializeField] GameObject qSkillLaserPrefab;

    [Header("W 스킬 UI")]
    [SerializeField] private Slider wSlider;
    [SerializeField] private TMP_Text wCooldownText;

    [Header("E 스킬 UI")]
    [SerializeField] private Slider eSlider;
    [SerializeField] private TMP_Text eCooldownText;

    [Header("R 스킬 UI")]
    [SerializeField] private Slider rSlider;
    [SerializeField] private TMP_Text rCooldownText;
    [SerializeField] private GameObject skillEffect;
    [SerializeField] private GameObject eyeLaserEffect;
    [SerializeField] public Transform attackPos;
    [SerializeField] public Vector2 attackBoxSize;
    // 마지막 사용 시간
    private float qLastUsed = -Mathf.Infinity;
    private float wLastUsed = -Mathf.Infinity;
    private float eLastUsed = -Mathf.Infinity;
    private float rLastUsed = -Mathf.Infinity;


    public Ghost ghost;
    public PlayerStat PlayerStat;
    public PlayerBase Player;

    private Coroutine ghostCoroutine;

    private void Update()
    {
        QSkill();
        WSkill();
        ESkill();
        RSkill();

        UpdateCooldownUI(qSlider, qCooldownText, qLastUsed, qCooldown);
        UpdateCooldownUI(wSlider, wCooldownText, wLastUsed, wCooldown);
        UpdateCooldownUI(eSlider, eCooldownText, eLastUsed, eCooldown);
        UpdateCooldownUI(rSlider, rCooldownText, rLastUsed, rCooldown);
    }

    private void QSkill()
    {
        if (!SkillManager.instance.QSkillActive) return;
        if (!CanUseSkill(qLastUsed, qCooldown)) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q 스킬 사용");
            AudioManager.instance?.PlaySound(transform.position, "서브레이저", Random.Range(1f, 1.1f), 1f);
            AudioManager.instance?.PlaySound(transform.position, "서브레이저2", Random.Range(1f, 1.1f), 1f);
            qSkillLaserPrefab.SetActive(false);
            qSkillLaserPrefab.SetActive(true);
            CameraShake.instance.ShakeCamera(8f, 0.3f);
            qLastUsed = Time.time;
        }
    }

    private void WSkill()
    {
        if (!SkillManager.instance.WSkillActive) return;
        if (!CanUseSkill(wLastUsed, wCooldown)) return;

        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("W 스킬 유지형 사용");
            wLastUsed = Time.time;
        }
    }

    private void ESkill()
    {
        if (!SkillManager.instance.ESkillActive) return;
        if (!CanUseSkill(eLastUsed, eCooldown)) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E 스킬 사용 - 순간이동");
            eLastUsed = Time.time;

            if (ghostCoroutine != null)
            {
                StopCoroutine(ghostCoroutine);
                ghostCoroutine = null;
            }

            // 새로 실행
            ghostCoroutine = StartCoroutine(Cor_ESkill());
        }
    }

    private IEnumerator Cor_ESkill()
    {
        SkillManager.instance.ESkilling = true;
        ghost.makeGhost = true;
        PlayerStat.moveSpeed *= 2f;
        yield return new WaitForSeconds(5f);
        ghost.makeGhost = false;
        SkillManager.instance.ESkilling = false;
        PlayerStat.moveSpeed /= 2f;

        ghostCoroutine = null;
    }

    private void RSkill()
    {
        if (!SkillManager.instance.RSkillActive) return;
        if (!CanUseSkill(rLastUsed, rCooldown)) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            eyeLaserEffect.SetActive(false);
            eyeLaserEffect.SetActive(true);
            AudioManager.instance?.PlaySound(transform.position, "서브레이저2", Random.Range(1f, 1.2f), 1f);
            AudioManager.instance?.PlaySound(transform.position, "RSkill", Random.Range(1f, 1.2f), 1f);
            GameManager.instance.Flash();
            Debug.Log("R 궁극기 발동");
            rLastUsed = Time.time;
            StartCoroutine(Cor_RSkill());
        }
    }
    private IEnumerator Cor_RSkill()
    {
        yield return new WaitForSeconds(0.5f);

        DialogManager.instance.isDialogActive = true;
        Player.spriteRenderer.enabled = false;

        skillEffect.SetActive(false);
        skillEffect.SetActive(true);
        for (int i = 0; i < 12; i++)
        {
            Damage(attackPos,attackBoxSize);
            AudioManager.instance?.PlaySound(transform.position, "Sword", Random.Range(1f, 1.2f), 1f);
            AudioManager.instance?.PlaySound(transform.position, "RskillSlash", Random.Range(1f, 1.1f), 1f);
            CameraShake.instance.ShakeCamera(4f, 0.2f);

            yield return new WaitForSeconds(0.095f);
        }
        DialogManager.instance.isDialogActive = false;
        Player.spriteRenderer.enabled = true;
    }

    private bool CanUseSkill(float lastUsedTime, float cooldown)
    {
        return Time.time >= lastUsedTime + cooldown;
    }

    private void UpdateCooldownUI(Slider slider, TMP_Text text, float lastUsedTime, float cooldown)
    {
        float elapsed = Time.time - lastUsedTime;
        float remaining = cooldown - elapsed;

        if (remaining > 0f)
        {
            slider.gameObject.SetActive(true);
            slider.value = Mathf.Clamp01(1f - (elapsed / cooldown));
            text.text = $"{remaining:F1}s";
        }
        else
        {
            slider.gameObject.SetActive(false); // 필요 시 숨기기
            text.text = " ";
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPos.position, attackBoxSize);
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
                    damageable.TakeDamage(Random.Range(PlayerStat.attackPower, PlayerStat.attackPower + 5));
                }

            }
        }
    }
}
