using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour,IDamageable
{
    public float hp = 10;
    [SerializeField] private Material hitMaterial;

    [SerializeField] private Material originalMaterial;
    [SerializeField] private SpriteRenderer spriteren;
    public void TakeDamage(float amount)
    {
        hp -= amount;
        StartCoroutine(Cor_HitMaterialChange());

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator Cor_HitMaterialChange()
    {
        spriteren.material = hitMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteren.material = originalMaterial;
    }
}
