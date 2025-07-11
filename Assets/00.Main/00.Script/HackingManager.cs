using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingManager : MonoBehaviour
{
    [SerializeField] private Hacking[] hakingList;
    [SerializeField] private HackingButton hakingButton;
    [SerializeField] private int currentIndex;

    [SerializeField] GameObject completeGo;
    [SerializeField] Door door;


    private void OnEnable()
    {
        isAlreadyComplete = false;

        // 시작 시 하나만 활성화
        for (int i = 0; i < hakingList.Length; i++)
        {
            hakingList[i].active = (i == currentIndex);
        }
    }
    private void Update()
    {
        HakingListMove();
        CheckAllComplete();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TimeLineManager.instance.isCutScene = false;
            hakingButton.hackingProgram.SetActive(false);
        }
    }

    public void HakingListMove()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 현재 항목 비활성화
            hakingList[currentIndex].active = false;
            AudioManager.instance?.PlaySound(transform.position, "Hacking", UnityEngine.Random.Range(1f, 1.1f), 1f);
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = hakingList.Length - 1;
            }

            // 새 항목 활성화
            hakingList[currentIndex].active = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // 현재 항목 비활성화
            hakingList[currentIndex].active = false;
            AudioManager.instance?.PlaySound(transform.position, "Hacking", UnityEngine.Random.Range(1f, 1.1f), 1f);
            currentIndex++;
            if (currentIndex >= hakingList.Length)
            {
                currentIndex = 0;
            }

            // 새 항목 활성화
            hakingList[currentIndex].active = true;
        }
    }

    public bool isAlreadyComplete = false; // 이미 완료되었는지 체크용

    private void CheckAllComplete()
    {
        bool allComplete = true;

        foreach (var hacking in hakingList)
        {
            if (!hacking.complete)
            {
                allComplete = false;
                break;
            }
        }

        completeGo.SetActive(allComplete);

        if (allComplete && !isAlreadyComplete)
        {
            isAlreadyComplete = true;

            foreach (var hacking in hakingList)
            {
                hacking.allcomple = true;
            }

            StartCoroutine(Cor_Flase());
        }
    }


    IEnumerator Cor_Flase()
    {
        StartCoroutine(door.Cor_DoorOpen());
        yield return new WaitForSeconds(2f);
        TimeLineManager.instance.isCutScene = false;
        hakingButton.hackingProgram.SetActive(false);
    }

   

}
