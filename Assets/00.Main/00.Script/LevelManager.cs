using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int currentLevel = 1;
    public float currentLevelValue = 0f; // ���� ����ġ
    public float maxLevelValue = 100f;   // �ʿ� ����ġ

    public float levelGrowthRate = 1.2f; // ���� ������ ������ �ʿ� ����ġ ���� (20%��)

    public Slider levelSlider;
    public TMP_Text levelText;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// ����ġ ȹ��
    /// </summary>
    public void AddExp(float amount)
    {
        currentLevelValue += amount;
        Debug.Log($"EXP ȹ��: {amount} / ����: {currentLevelValue} / �ʿ�: {maxLevelValue}");
        CheckLevelUp();
        levelSlider.value = currentLevelValue/maxLevelValue;
    }

    /// <summary>
    /// ������ ���� üũ
    /// </summary>
    private void CheckLevelUp()
    {
        if (currentLevelValue >= maxLevelValue)
        {
            currentLevelValue -= maxLevelValue; // ���� ����ġ�� ���� ������ �̿�
            currentLevel++;
            maxLevelValue *= levelGrowthRate;
            levelText.text = "Lv." +  currentLevel.ToString();   
            Debug.Log($"���� ��! ���� ����: {currentLevel} / ���� �ʿ� ����ġ: {maxLevelValue}");
        }
    }
}
