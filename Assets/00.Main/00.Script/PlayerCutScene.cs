using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCutScene : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if (collision.gameObject.CompareTag("CutScene"))
            {
                collision.gameObject.SetActive(false);
                TimeLineManager.instance.StartCutScene(0);
            }
        }
    }
}
