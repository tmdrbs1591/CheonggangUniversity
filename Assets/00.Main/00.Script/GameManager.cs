using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject Menu;
    bool isMenu;
    public TMP_Text stageText;

    public PlayerBase playerCont;
    public GameObject flash;
    public Transform itemUIPos;
    public GameObject interactionUI;
    public GameObject riderBossHpSlider;
    public GameObject riderBossBaseHpSlider;
    public GameObject hackerBossHpSlider;
    public GameObject hackerBossBaseHpSlider;
    public GameObject miniBossHpSlider;
    public GameObject miniBossBaseHpSlider;
    public GameObject electricBossHpSlider;
    public GameObject electricBossBaseHpSlider;
    public RiderBoss riderboss;
    public HackerBoss hackerBoss;
    public ElectricMiniBoss miniboss;
    public ElectricBoss eletricBoss;

    public GameObject hackingProgram;

    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isMenu)
        {
            Menu.SetActive(true);
            isMenu = true;
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && isMenu)
        {
            Menu.SetActive(false);
            isMenu = false;
        }
    }
        public void MiniBossFalse()
    {
        
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
        SongManager.instance.SongChange(1);
    }

    public void HakcerBossHpSliderActive()
    {
        hackerBossHpSlider.SetActive(true);
        hackerBossBaseHpSlider.SetActive(true);
        hackerBoss.isBattle = true;
        SongManager.instance.SongChange(1);

    }

    public void MiniBossHpSliderActive()
    {
        miniBossHpSlider.SetActive(true);
        miniBossBaseHpSlider.SetActive(true);
        miniboss.isBattle = true;
        SongManager.instance.SongChange(1);

    }

    public void ElectricBossHpSliderActive()
    {
        miniboss.gameObject.SetActive(false);
        eletricBoss.gameObject.SetActive(true);
        SongManager.instance.SongChange(2);

        StartCoroutine(Cor_true());
    }
    IEnumerator Cor_true()
    {
        yield return new WaitForSeconds(0.2f);
        electricBossHpSlider.SetActive(true);
        electricBossBaseHpSlider.SetActive(true);
        eletricBoss.isBattle = true;
    }
}
