using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.Utility;
using Cinemachine;

public class CameraAreaScript : MonoBehaviour
{
    [SerializeField]CinemachineConfiner2D confiner;
  

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            confiner.m_BoundingShape2D = gameObject.GetComponent<PolygonCollider2D>();


    }
}
