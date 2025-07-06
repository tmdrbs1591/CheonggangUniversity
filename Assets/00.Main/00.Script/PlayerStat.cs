using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    [Header("Stat Settings")]
    public float moveSpeed = 5f;
    public float maxHp;
    public float currentHp;
    public float maxMana;
    public float currentMana;
    public int attackPower;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text manaText;

    [Header("Mana Regen")]
    [SerializeField] private float manaRegenSpeed = 5f; // 초당 마나 회복량

    private void Start()
    {
        StartCoroutine(ManaRegenCoroutine());
    }

    private IEnumerator ManaRegenCoroutine()
    {
        while (true)
        {
            if (currentMana < maxMana)
            {
                currentMana += manaRegenSpeed * Time.deltaTime;
                currentMana = Mathf.Min(currentMana, maxMana);
                UpdateUI();
            }
            yield return null;
        }
    }

    public void IncreaseHealth(float v)
    {
        currentHp += v;
        currentHp = Mathf.Min(currentHp, maxHp);
    }

    public void ManaUse(float val)
    {
        currentMana -= val;
        UpdateUI(); 
    }
    public void UpdateUI()
    {
        hpSlider.value = currentHp / maxHp;
        hpText.text = $"{Mathf.FloorToInt(currentHp)}/{maxHp}";

        manaSlider.value = currentMana / maxMana;
        manaText.text = $"{Mathf.FloorToInt(currentMana)}/{maxMana}";
    }
}
