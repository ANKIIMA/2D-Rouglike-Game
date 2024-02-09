using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBaseSO : ScriptableObject
{
    [Header("��������")]
    public string m_UnitName;
    public string m_type;

    public int m_levelhp;
    public int m_maxhp;

    public int m_levelsp;
    public int m_maxsp;

    public int m_levelAttack;
    public int m_attackValue;

    [Header("Ԥ�Ƽ�")]
    public GameObject m_prefab;

    [Header("ͷ��")]
    public Sprite avatar;

}
