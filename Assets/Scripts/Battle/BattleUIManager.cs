using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager instance;
    private GameObject attacker;
    //Transfrom of Battle UI Manager.
    private Transform actionPanel;
    //Actoin index.
    private int actionIndex;
    //Enemy Target
    private BattleEnemy enemyTarget;

    private void Awake()
    {

        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(instance);
        }
        actionPanel = GameObject.Find("ActionPanel").transform;

        actionIndex = -1;
    }

    private void OnEnable()
    {
        actionIndex = -1;
    }

    public void OnButtonAttack1()
    {
        //Debug.Log("Attack1");
        actionIndex = 0;
        Debug.Log("current index" +  actionIndex);
    }

    public void OnButtonAttack2()
    {
        //Debug.Log("Attack2");
        actionIndex = 1;
        Debug.Log("current index" + actionIndex);
    }

    public void OnButtonSkill1()
    {
        //Debug.Log("Pass");
        actionIndex = 2;
        Debug.Log("current index" + actionIndex);
    }
    /// <summary>
    /// set the target enemy;
    /// </summary>
    /// <param name="enemy"></param>
    public void OnEnemyChose(BattleEnemy enemy)
    {
        enemyTarget = enemy;
    }

    /// <summary>
    /// Confirm to act.
    /// </summary>
    public void OnButtonConfirm()
    {
        if (actionIndex < 0 || actionIndex > 3)
        {
            Debug.Log("Please choose the action. Current Index: " + actionIndex);
        }
        else
        {
            //set the config of player.
            BattleStateManager.instance.heroAttackState.SetCode(actionIndex);
            BattleStateManager.instance.heroAttackState.SetTarget(enemyTarget);
           BattleStateManager.instance.OnChangeState(BattleStateManager.instance.heroAttackState);

        }
    }

    public void DeactivateActionPanel()
    {
        actionPanel.gameObject.SetActive(false);
    }

    public void ActivateActionPanel()
    {
        actionPanel.gameObject.SetActive(true);
    }
}
