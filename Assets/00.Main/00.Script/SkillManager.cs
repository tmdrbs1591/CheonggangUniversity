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

    public GameObject firePasivSkill;
    public GameObject hpPasivSkill;
    public GameObject doubleAttackPasivSkill;
    public GameObject dashManaPasivSkill;

    public bool QSkillActive;
    public bool WSkillActive;
    public bool ESkillActive;
    public bool RSkillActive;

    public bool ESkilling;

    public bool firePasivSkillActive;
    public bool hpPasivSkillActive;
    public bool doubleAttackPasivSkillActive;
    public bool dashManaPasivSkillActive;

    private void Awake()
    {
        instance = this;
    }
}
