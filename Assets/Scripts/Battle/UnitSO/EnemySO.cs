using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySO : UnitBaseSO
{
    [Serializable]
    public struct Skill
    {
        //技能名
        public string m_SkillName;
        //技能伤害/回复
        public int m_SkillDamage;
        //音效
        public AudioClip m_ActionSound;
    }

    [SerializeField]
    List<Skill> m_Skills;
}
