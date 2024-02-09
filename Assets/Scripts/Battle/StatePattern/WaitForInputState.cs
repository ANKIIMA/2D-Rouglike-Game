using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForInputState : BattleState
{
    BattleHero currentUnit;
    public void OnStateAction()
    {
        
    }

    public void OnStateEnter()
    {
        //activate the button
        BattleUIManager.instance.ActivatePlayerActionPanel();

        currentUnit = BattleManager.instance.GetActionHero();
        if(currentUnit == null)
        {
            return;
        }
        //change the avatar position to the unit.   
        BattleUIManager.instance.UpdateHeroInfo(currentUnit.GetPosition(), currentUnit);
    }

    public void OnStateLeave()
    {
        
        //deactivate the button
        BattleUIManager.instance.DeactivatePlayerActionPanel();

    }

    public BattleHero GetCurrentUnit()
    {
        return currentUnit;
    }
}
