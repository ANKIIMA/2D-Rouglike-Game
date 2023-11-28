using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.UI;

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

    public void DeactivatePlayerActionPanel()
    {
        actionPanel.transform.Find("playerInputInfo").gameObject.SetActive(false);
    }

    public void ActivatePlayerActionPanel()
    {
        actionPanel.transform.Find("playerInputInfo").gameObject.SetActive(true);
    }

    public void ActivateEnemyInfo()
    {
        actionPanel.transform.Find("EnemyInfo").gameObject.SetActive(true);
    }

    public void DeactivateEnemyInfo()
    {
        actionPanel.transform.Find("EnemyInfo").gameObject.SetActive(false);
    }


    /// <summary>
    /// update the ui info of hero
    /// </summary>
    /// <param name="x">position of hero</param>
    /// <param name="unit">script of hero</param>
    public void UpdateHeroInfo(float x, BattleHero unit)
    {
        //camera and avatar
        Transform avatarCamera = GameObject.Find("HeroCamera").transform;
        avatarCamera.position = new Vector3(x + 0.24f, avatarCamera.position.y, avatarCamera.position.z);
        avatarCamera.SetParent(unit.transform);

        //health bar and skill bar
        Slider healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        Slider skillBar = GameObject.Find("SkillBar").GetComponent<Slider>();
        if (healthBar != null)
        {
            healthBar.value = unit.GetHealthValue();
            healthBar.interactable = false;
        }
        if(skillBar != null)
        {
            skillBar.value = unit.GetSkillValue();
            skillBar.interactable = false;
        }

        //name
        Text heroName = GameObject.Find("UnitName").GetComponent<Text>();
        if(heroName != null)
        {
            heroName.text = unit.name;
        }

        //Skill Button name
    }

    public void UpdateEnemyInfo()
    {

    }
}
