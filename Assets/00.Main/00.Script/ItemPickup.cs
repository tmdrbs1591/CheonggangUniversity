using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public float launchForce = 5f; // Æ¢´Â Èû ¼¼±â Á¶Àý
    bool isPickup;

    private void Start()
    {
        StartCoroutine(Cor_IsPickUp());
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDir * launchForce, ForceMode2D.Impulse);
        }
    }

    IEnumerator Cor_IsPickUp()
    {
        yield return new WaitForSeconds(0.2f);
        isPickup = true;
    }
    public void Pickup()
    {
        if (!isPickup)
            return;
        if (item.id == 0)
        {
            InventoryManager.instance.Add(item);
            SetUI();
        }
        else if (item.id == 1)
        {
            SetUI();
            switch (item.itemType)
            {
                case ItemType.QSkill:
                    SkillManager.instance.QSkill.gameObject.SetActive(true);
                    SkillManager.instance.QSkillActive = true;
                    break;
                case ItemType.WSkill:
                    SkillManager.instance.WSkill.gameObject.SetActive(true);
                    SkillManager.instance.WSkillActive = true;
                    break;
                case ItemType.ESkill:
                    SkillManager.instance.ESkill.gameObject.SetActive(true);
                    SkillManager.instance.ESkillActive = true;
                    break;
                case ItemType.RSkill:
                    SkillManager.instance.RSkill.gameObject.SetActive(true);
                    SkillManager.instance.RSkillActive = true;
                    break;
                case ItemType.FirePasivSkill:
                    SkillManager.instance.firePasivSkill.gameObject.SetActive(true);
                    SkillManager.instance.firePasivSkillActive = true;
                    break;
                case ItemType.HpPasivSkill:
                    SkillManager.instance.hpPasivSkill.gameObject.SetActive(true);
                    SkillManager.instance.hpPasivSkillActive = true;
                    break;
                case ItemType.DoubleAttackPasivSkill:
                    SkillManager.instance.doubleAttackPasivSkill.gameObject.SetActive(true);
                    SkillManager.instance.doubleAttackPasivSkillActive = true;
                    break;
                case ItemType.DashManaPasivSkill:
                    SkillManager.instance.dashManaPasivSkill.gameObject.SetActive(true);
                    SkillManager.instance.dashManaPasivSkillActive = true;
                    break;
            }
        }
        Destroy(gameObject);

    }

    private void SetUI()
    {
        GameObject itemUI = ObjectPool.SpawnFromPool("ItemUI", GameManager.instance.itemUIPos.position);
        itemUI.transform.SetParent(GameManager.instance.itemUIPos, false);
        var itemName = itemUI.transform.Find("ItemName").GetComponent<TMP_Text>();
        var itemIcon = itemUI.transform.Find("ItemIcon").GetComponent<Image>();
        AudioManager.instance?.PlaySound(transform.position, "item", UnityEngine.Random.Range(1f, 1.2f), 1f);
        itemName.text = item.itemName + " X1";
        itemIcon.sprite = item.icon;
    }
}
