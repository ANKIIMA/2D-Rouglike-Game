using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttackState : BattleState
{
    private BattleHero currentUnit;
    public void OnStateAction()
    {
        Debug.Log("Hero Attack.");
    }

    public void OnStateEnter()
    {
        Debug.Log("Enter Hero Attack.");
    }

    public void OnStateLeave()
    {
        Debug.Log("Hero Attack Done.");
    }

}
