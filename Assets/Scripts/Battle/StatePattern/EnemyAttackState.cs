using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : BattleState
{
    private BattleEnemy currentUnit;
    public void OnStateAction()
    {
        Debug.Log("Enemy Attack.");
    }

    public void OnStateEnter()
    {
        Debug.Log("Enter Enemy Attack.");
    }

    public void OnStateLeave()
    {
        Debug.Log("Enemy Attack Done.");
    }
}
