using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBaseSO : UnitBaseSO
{
    [Serializable]
    public struct Skill
    {
        //技能名
        public string m_SkillName;
        //技能描述
        public string m_skillDescription;
        //技能伤害/回复
        public int m_SkillDamage;
        //音效
        public AudioClip m_ActionSound;
    }

    [Header("技能配置")]
    public List<Skill> m_SkillList;


}
