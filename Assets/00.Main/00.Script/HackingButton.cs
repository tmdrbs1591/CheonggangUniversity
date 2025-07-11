using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HackingButton : MonoBehaviour, Interaction
{
    [SerializeField] public GameObject hackingProgram;
    public HackingManager hackingManager;
    public void EventStart()
    {if (hackingManager.isAlreadyComplete)
            return;
       hackingProgram.SetActive(true);
        TimeLineManager.instance.isCutScene = true;
    }
}
