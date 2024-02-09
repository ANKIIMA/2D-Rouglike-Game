using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BattleKnight : BattleHero
{
    protected override void Awake()
    {
        base.Awake();
        LoadData();
    }

    private void LoadData()
    {
        KnightSO data = (KnightSO)Resources.Load("SOAssets/KnightData");
        //initiate hp and sp
        m_maxhp = data.m_maxhp;
        m_hp = m_maxhp;
        m_maxsp = data.m_maxsp;
        m_sp = m_maxsp;

        m_attackValue = data.m_attackValue;

        m_type = data.m_type;
    }
}
