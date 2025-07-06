using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public GameObject QSkill;
    public GameObject WSkill;
    public GameObject ESkill;
    public GameObject RSkill;

    public bool QSkillActive;
    public bool WSkillActive;
    public bool ESkillActive;
    public bool RSkillActive;

    public bool ESkilling;

    private void Awake()
    {
        instance = this;
    }
}
