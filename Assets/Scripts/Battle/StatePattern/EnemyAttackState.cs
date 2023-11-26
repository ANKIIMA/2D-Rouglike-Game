using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : BattleState
{
    private BattleEnemy currentUnit;
    private int actionCount;
    private bool allActionDone = false;
    public void OnStateAction()
    {
        unitAction();
        //all the units haved attacked.
        if(allActionDone == true)
        {
            //change to wait for input state;
            BattleStateManager.instance.OnChangeState(BattleStateManager.instance.waitForInputState);
;        }
        

    }

    public void OnStateEnter()
    {
        Debug.LogError("Enter Enemy Attack.");
        allActionDone = false;
        actionCount = 0;

        //get new unit.
        currentUnit = BattleManager.instance.GetActionEnemy();
        actionCount++;

        //deactivate the ActionPanel
        BattleUIManager.instance.DeactivateActionPanel();

        //clear the unitIndex for hero
        BattleManager.instance.ClearUnitIndex();

        currentUnit.StartTimer();
    }

    public void OnStateLeave()
    {
        //activate the actionPanel
        BattleUIManager.instance.ActivateActionPanel();
    }

    private void unitAction()
    {
        //action is not over.
        if(currentUnit.Attack() == false)
        {
            return;
        }
        //action is over
        else
        {
            currentUnit.ResetActionDone();
            //all unit have attacked
            if(actionCount == BattleManager.instance.GetEnemyCount())
            {
                allActionDone = true;
            }
            //there is other units which did not act
            else
            {
                currentUnit = BattleManager.instance.GetActionEnemy();
                actionCount++;
                
                currentUnit.StartTimer();
            }
        }
    }
}
