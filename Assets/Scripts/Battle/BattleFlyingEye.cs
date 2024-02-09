using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BattleFlyingEye : BattleEnemy
{
    protected override void Awake()
    {
        base.Awake();

        LoadData();
    }

    private void LoadData()
    {
        FlyingEyeSO data = (FlyingEyeSO)Resources.Load("SOAssets/FlyingEyeData");

        m_maxhp = data.m_maxhp;
        m_hp = m_maxhp;

        m_maxsp = data.m_maxsp;
        m_sp = m_maxsp;

        m_attackValue = data.m_attackValue;

    }
}
