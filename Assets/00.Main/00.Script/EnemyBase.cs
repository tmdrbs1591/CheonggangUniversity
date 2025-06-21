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
        CameraShake.instance.ShakeCamera(5f, 0.15f);

       

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        ObjectPool.SpawnFromPool("DieEffect", transform.position);
        Destroy(gameObject);
    }

    private IEnumerator Cor_HitMaterialChange()
    {
        spriteren.material = hitMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteren.material = originalMaterial;
    }
}
