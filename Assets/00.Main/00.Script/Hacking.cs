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
    public bool allcomple;

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
        if (allcomple)
            return;
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentIndex--;
            AudioManager.instance?.PlaySound(transform.position, "Hacking", UnityEngine.Random.Range(1f, 1.1f), 1f);
            if (currentIndex < 0)
            {
                currentIndex = 4;
            }
            image.sprite = hackSprite[currentIndex];
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentIndex++;
            AudioManager.instance?.PlaySound(transform.position, "Hacking", UnityEngine.Random.Range(1f, 1.1f), 1f);
            if (currentIndex >= hackSprite.Length)
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

