using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySO : UnitBaseSO
{
    [Serializable]
    public struct Skill
    {
        //������
        public string m_SkillName;
        //�����˺�/�ظ�
        public int m_SkillDamage;
        //��Ч
        public AudioClip m_ActionSound;
    }

    [SerializeField]
    List<Skill> m_Skills;
}
