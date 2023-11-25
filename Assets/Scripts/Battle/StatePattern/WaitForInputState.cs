using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForInputState : BattleState
{
    public void OnStateAction()
    {
        //Debug.Log("Wait For Input.");
    }

    public void OnStateEnter()
    {
        Debug.Log("Start to Wait For Input.");
    }

    public void OnStateLeave()
    {
        Debug.Log("Input Done.");
    }
}
