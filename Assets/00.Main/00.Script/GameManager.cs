using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerBase playerCont;
    public GameObject flash;
    public Transform itemUIPos;
    public GameObject interactionUI;
    public GameObject riderBossHpSlider;
    public GameObject riderBossBaseHpSlider;
    public GameObject hackerBossHpSlider;
    public GameObject hackerBossBaseHpSlider;
    public RiderBoss riderboss;
    public HackerBoss hackerBoss;

    public GameObject hackingProgram;

    private void Awake()
    {
        instance = this;
    }
    public void Flash()
    {
        flash.SetActive(false);
        flash.SetActive(true);
    }
    public void RiderBossHpSliderActive()
    {
        riderBossHpSlider.SetActive(true);
        riderBossBaseHpSlider.SetActive(true);
        riderboss.isBattle = true;  
    }

    public void HakcerBossHpSliderActive()
    {
        hackerBossHpSlider.SetActive(true);
        hackerBossBaseHpSlider.SetActive(true);
        hackerBoss.isBattle = true;

    }
}
