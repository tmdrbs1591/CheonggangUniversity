using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAddUI : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.Euler(-180, 0, 0);
    }
    }
