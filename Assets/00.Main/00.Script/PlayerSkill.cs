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

    [Header("W 스킬 UI")]
    [SerializeField] private Slider wSlider;
    [SerializeField] private TMP_Text wCooldownText;

    [Header("E 스킬 UI")]
    [SerializeField] private Slider eSlider;
    [SerializeField] private TMP_Text eCooldownText;

    [Header("R 스킬 UI")]
    [SerializeField] private Slider rSlider;
    [SerializeField] private TMP_Text rCooldownText;

    // 마지막 사용 시간
    private float qLastUsed = -Mathf.Infinity;
    private float wLastUsed = -Mathf.Infinity;
    private float eLastUsed = -Mathf.Infinity;
    private float rLastUsed = -Mathf.Infinity;


    public Ghost ghost;
    public PlayerStat PlayerStat;

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
            Debug.Log("R 궁극기 발동");
            rLastUsed = Time.time;
        }
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
}
