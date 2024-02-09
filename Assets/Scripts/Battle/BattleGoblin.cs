using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleGoblin : BattleEnemy
{
    protected override void Awake()
    {
        base.Awake();

        LoadData();
    }

    private void LoadData()
    {
        GoblinSO data = (GoblinSO)Resources.Load("SOAssets/GoblinData");

        m_maxhp = data.m_maxhp;
        m_hp = m_maxhp;

        m_maxsp = data.m_maxsp;
        m_sp = m_maxsp;

        m_attackValue = data.m_attackValue;

    }
}
