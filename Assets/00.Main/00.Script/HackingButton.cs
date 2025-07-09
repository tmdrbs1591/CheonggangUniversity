using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HackingButton : MonoBehaviour, Interaction
{
    public void EventStart()
    {
        GameManager.instance.hackingProgram.SetActive(true);
    }
}
