using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackingManager : MonoBehaviour
{
    [SerializeField] private Hacking[] hakingList;
    [SerializeField] private int currentIndex;

    [SerializeField] GameObject completeGo;


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
    }

    public void HakingListMove()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            // 현재 항목 비활성화
            hakingList[currentIndex].active = false;

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

            currentIndex++;
            if (currentIndex >= hakingList.Length)
            {
                currentIndex = 0;
            }

            // 새 항목 활성화
            hakingList[currentIndex].active = true;
        }
    }

    private bool isAlreadyComplete = false; // 이미 완료되었는지 체크용

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

        // 모든 complete가 true일 때 딱 한 번만 cor = false 실행
        if (allComplete && !isAlreadyComplete)
        {
            isAlreadyComplete = true;
            StartCoroutine(Cor_Flase());
        }
    }


    IEnumerator Cor_Flase()
    {
        yield return new WaitForSeconds(2f);
        GameManager.instance.hackingProgram.SetActive(false);
    }
}
