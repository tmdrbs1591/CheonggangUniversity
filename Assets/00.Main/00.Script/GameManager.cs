using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public PlayerBase playerCont;
    public GameObject flash;
    public Transform itemUIPos;
    public GameObject interactionUI;

    private void Awake()
    {
        instance = this;
    }
    public void Flash()
    {
        flash.SetActive(false);
        flash.SetActive(true);
    }
}
