using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectReturn : MonoBehaviour
{
    [SerializeField] private string name;

    private void Start()
    {
        StartCoroutine(Cor_ReturnToPool(name));
    }
    IEnumerator Cor_ReturnToPool(string nm)
    {
        yield return new WaitForSeconds(2f);
        ObjectPool.ReturnToPool(name, gameObject);
    }
}
