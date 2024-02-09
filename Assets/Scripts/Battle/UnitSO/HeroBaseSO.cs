using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBaseSO : UnitBaseSO
{
    [Serializable]
    public struct Skill
    {
        //������
        public string m_SkillName;
        //��������
        public string m_skillDescription;
        //�����˺�/�ظ�
        public int m_SkillDamage;
        //��Ч
        public AudioClip m_ActionSound;
    }

    [Header("��������")]
    public List<Skill> m_SkillList;


}
