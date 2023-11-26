using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateManager : MonoBehaviour
{
    public static BattleStateManager instance;
    private BattleState currentState;

    [NonSerialized]
    public HeroAttackState heroAttackState;
    [NonSerialized]
    public EnemyAttackState enemyAttackState;
    [NonSerialized]
    public WaitForInputState waitForInputState;

    //Singleton
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
        {
            Destroy(instance);
        }

        //Create states to avoid another new
        heroAttackState = new HeroAttackState();
        
        enemyAttackState = new EnemyAttackState();
        waitForInputState = new WaitForInputState();

    }

    /// <summary>
    /// Initiate to the WaitForInput State.
    /// </summary>
    private void OnEnable()
    {
        currentState = waitForInputState;
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
}
