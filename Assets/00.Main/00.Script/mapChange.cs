using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class mapChange : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Sprite changeSprite;
    [SerializeField] private string stageName;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            background.sprite = changeSprite;
            GameManager.instance.stageText.gameObject.SetActive(false);
            GameManager.instance.stageText.gameObject.SetActive(true);
            GameManager.instance.stageText.text = stageName;
        }
    }
}
