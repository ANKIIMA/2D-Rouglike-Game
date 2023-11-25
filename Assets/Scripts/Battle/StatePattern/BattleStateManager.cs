using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateManager : MonoBehaviour
{
    public static BattleStateManager instance;
    private BattleState currentState;

    //Singleton
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
        {
            Destroy(instance);
        }
    }

    /// <summary>
    /// Initiate to the WaitForInput State.
    /// </summary>
    private void OnEnable()
    {
        currentState = new WaitForInputState();
        currentState.OnStateEnter();
    }

    /// <summary>
    /// Do the current state actions.
    /// </summary>
    private void Update()
    {
        currentState.OnStateAction();
    }

    /// <summary>
    /// change the current state.
    /// </summary>
    /// <param name="newState">new state</param>
    public void OnChangeState(BattleState newState)
    {
        currentState.OnStateLeave();
        currentState = newState;
        currentState.OnStateEnter();

    }

    public Type GetCurrentState()
    {
        return currentState.GetType();
    }
}
