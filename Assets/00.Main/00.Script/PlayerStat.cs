using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{

    [Header("Stat Settings")]
    public float moveSpeed = 5f;
    public float maxHp;
    public float currentHp;
    public int attackPower;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;
    public void IncreaseHealth(float v)
    {
        currentHp += v;
    }
    public void UpdateUI()
    {
        hpSlider.value = currentHp / maxHp;
        hpText.text = $"{currentHp}/{maxHp}";

    }
}
