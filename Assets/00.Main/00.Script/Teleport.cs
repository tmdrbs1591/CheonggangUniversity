using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField] private Transform targetPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(Cor_Teleport(collision));
        }
    }

            
    IEnumerator Cor_Teleport(Collider2D collision)
    {
        FadeManager.instance.FadeInOut();
        yield return new WaitForSeconds(1f);
        collision.transform.position = targetPosition.transform.position;
        gameObject.SetActive(false);

    }
}
