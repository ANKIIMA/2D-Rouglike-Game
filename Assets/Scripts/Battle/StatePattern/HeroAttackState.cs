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
        currentUnit = BattleManager.instance.GetActionHero();
        BattleUIManager.instance.DeactivateActionPanel();
    }

    public void OnStateLeave()
    {
        //Debug.Log("Hero Attack Done.");

        BattleUIManager.instance.ActivateActionPanel();
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
