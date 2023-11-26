using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForInputState : BattleState
{
    public void OnStateAction()
    {
        
    }

    public void OnStateEnter()
    {
        Debug.LogError("Start to Wait For Input.");
        //activate the button
        BattleUIManager.instance.ActivateActionPanel();
    }

    public void OnStateLeave()
    {
        
        //deactivate the button
        BattleUIManager.instance.DeactivateActionPanel();

    }
}
