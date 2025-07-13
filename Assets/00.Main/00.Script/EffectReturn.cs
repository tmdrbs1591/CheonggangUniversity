using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectReturn : MonoBehaviour
{
    [SerializeField] private string name;
    public float time = 2f;

    private void OnEnable()
    {
        StartCoroutine(Cor_ReturnToPool(name));
    }
    IEnumerator Cor_ReturnToPool(string nm)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.ReturnToPool(name, gameObject);
    }
}
