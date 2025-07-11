using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialChest : MonoBehaviour
{
    public bool isOpen = false;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private GameObject skill;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Open()
    {
        spriteRenderer.sprite = openSprite;
        Instantiate(skill, transform.position, Quaternion.identity);

        isOpen = true;
    }
    
}
