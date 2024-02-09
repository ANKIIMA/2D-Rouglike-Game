using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttackState : BattleState
{
    //hero unit script
    private BattleHero currentUnit;
    //which action to choose
    private int actionCode;
    //Attack target enemy
    private BattleEnemy targetEnemy;
    public void OnStateAction()
    {
        
        //if action is over.
        if (currentUnit.Action(actionCode, targetEnemy) == true)
        {
            currentUnit.ResetBool();
            //index ++ for the next hero.
            BattleManager.instance.AddUnitIndex();
            //there is more heros who did not act.  
            if (BattleManager.instance.GetHeroCount() > BattleManager.instance.unitIndex)
            {
                //wait for input
                BattleStateManager.instance.OnChangeState(BattleStateManager.instance.waitForInputState);
            }
            //there is no heros who did not act.
            else
            {
                //enemy attack.
                BattleStateManager.instance.OnChangeState(BattleStateManager.instance.enemyAttackState);
            }
        }
    }

    public void OnStateEnter()
    {
        Debug.LogError("Enter Hero Attack.");
        //get current unit.
        currentUnit = BattleStateManager.instance.waitForInputState.GetCurrentUnit();
        BattleUIManager.instance.DeactivatePlayerActionPanel();
        //start the timer to stimulate.
        //currentUnit.StartTimer();

    }

    public void OnStateLeave()
    {
        BattleUIManager.instance.ActivatePlayerActionPanel();
        if (BattleManager.instance.GetEnemyCount() <= 0)
        {
            GameManager.instance.ExitBattle();
        }
    }

    /// <summary>
    /// Set the action code.
    /// </summary>
    /// <param name="num">action code</param>
    public void SetCode(int num)
    {
        actionCode = num;
    }

    public void SetTarget(BattleEnemy target)
    {
        targetEnemy = target;
    }

}
