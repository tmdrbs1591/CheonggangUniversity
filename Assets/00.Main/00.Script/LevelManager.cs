using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int currentLevel = 1;
    public float currentLevelValue = 0f; // 현재 경험치
    public float maxLevelValue = 100f;   // 필요 경험치

    public float levelGrowthRate = 1.2f; // 다음 레벨로 갈수록 필요 경험치 증가 (20%씩)

    public Slider levelSlider;
    public TMP_Text levelText;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 경험치 획득
    /// </summary>
    public void AddExp(float amount)
    {
        currentLevelValue += amount;
        Debug.Log($"EXP 획득: {amount} / 총합: {currentLevelValue} / 필요: {maxLevelValue}");
        CheckLevelUp();
        levelSlider.value = currentLevelValue/maxLevelValue;
    }

    /// <summary>
    /// 레벨업 조건 체크
    /// </summary>
    private void CheckLevelUp()
    {
        if (currentLevelValue >= maxLevelValue)
        {
            currentLevelValue -= maxLevelValue; // 남은 경험치는 다음 레벨로 이월
            currentLevel++;
            maxLevelValue *= levelGrowthRate;
            levelText.text = "Lv." +  currentLevel.ToString();   
            Debug.Log($"레벨 업! 현재 레벨: {currentLevel} / 다음 필요 경험치: {maxLevelValue}");
        }
    }
}
