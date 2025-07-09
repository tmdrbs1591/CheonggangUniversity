using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mapChange : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Sprite changeSprite;


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            background.sprite = changeSprite;
        }
    }
}
