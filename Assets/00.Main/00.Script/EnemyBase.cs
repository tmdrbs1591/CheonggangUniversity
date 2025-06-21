using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] private int hp = 10;

    [SerializeField] private Material hitMaterial;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private SpriteRenderer spriteren;

    public void TakeDamage(int amount)
    {
        hp -= amount;
        StartCoroutine(Cor_HitMaterialChange());
        Debug.Log($"Enemy damaged! HP: {hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }

    private IEnumerator Cor_HitMaterialChange()
    {
        spriteren.material = hitMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteren.material = originalMaterial;
    }
}
