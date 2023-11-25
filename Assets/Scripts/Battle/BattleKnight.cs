using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class BattleKnight : BattleHero
{
    public GameObject target;

    private void OnEnable()
    {
        target = GameObject.Find("FlyingEye"); 
    }

}
