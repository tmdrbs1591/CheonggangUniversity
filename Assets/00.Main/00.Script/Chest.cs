     using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Chest : MonoBehaviour
    {
        public bool isOpen = false;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite openSprite;
        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void Open()
        {
            spriteRenderer.sprite = openSprite;
            for (int i = 0; i < 3; i++) 
            SpawnRandomItem(transform.position);

            isOpen = true;
        }

        protected void SpawnRandomItem(Vector2 spawnPos)
        {
            if (isOpen)
                return;
        
            if (InventoryManager.instance.itemPrefabs == null || InventoryManager.instance.itemPrefabs.Length == 0)
            {
                Debug.LogWarning("아이템 프리팹이 없습니다.");
                return;
            }

            int randIndex = Random.Range(0, InventoryManager.instance.itemPrefabs.Length);
            GameObject prefab = InventoryManager.instance.itemPrefabs[randIndex];
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
