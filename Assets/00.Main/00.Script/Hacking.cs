using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hacking : MonoBehaviour
{
    public int completeIndex;
    public int currentIndex;

    [SerializeField] Image image;
    [SerializeField] Sprite[] hackSprite;
    [SerializeField] GameObject arrow;

    public bool active;
    public bool complete;

    private void OnEnable()
    {
        int randInex = Random.Range(0, 5);
        currentIndex = randInex;
        image.sprite = hackSprite[currentIndex];

    }
    private void Update()
    {
        if (active) {
            arrow.SetActive(true);
            IndexMove();
        }
        else
        {
            arrow.SetActive(false);
        }


    }
    void IndexMove()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 4;
            }
            image.sprite = hackSprite[currentIndex];
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentIndex++;
            if (currentIndex > 4)
            {
                currentIndex = 0;
            }
            image.sprite = hackSprite[currentIndex];
        }

        // completeIndex와 currentIndex가 같을 경우 complete = true
        if (currentIndex == completeIndex)
        {
            complete = true;
        }
        else
        {
            complete = false;
        }
    }



}

