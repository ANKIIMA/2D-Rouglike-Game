using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.CanvasScaler;
using Slider = UnityEngine.UI.Slider;



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
        //singleton
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(instance);
        }
        //ui enable
        actionPanel = GameObject.Find("ActionPanel").transform;
        //action index initiate
        actionIndex = -1;
        //enemy info initiate
        enemyTarget = null;
        DeactivateEnemyInfo();

    }

    private void OnEnable()
    {
        actionIndex = -1;
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if( mouse == null )
        {
            return;
        }
        else if(mouse.leftButton.isPressed)
        {
            //ScreenPoint
            var onScreenPosition = mouse.position.ReadValue();
            //Ray(from camera to point in world space, vertical in 2D)
            var ray = Camera.main.ScreenPointToRay(onScreenPosition);
            //ray cast
            var hit = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, Mathf.Infinity);

            if(hit.collider != null )
            {
                //set target
                enemyTarget = hit.collider.GetComponent<BattleEnemy>();
                //ui update
                ActivateEnemyInfo();
                UpdateEnemyInfo(enemyTarget);
            }
        }
        
    }

    public void OnButtonAttack1()
    {
        actionIndex = 0;
    }

    public void OnButtonAttack2()
    {
        actionIndex = 1;
    }

    public void OnButtonSkill1()
    {
        actionIndex = 2;
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
        //Bar
        UpdateHeroBar(unit);
        //name
        Text heroName = GameObject.Find("UnitName").GetComponent<Text>();
        if(heroName != null)
        {
            heroName.text = unit.name;
        }

        //TODO: Skill Button name
    }

    /// <summary>
    /// //health bar and skill bar
    /// </summary>
    /// <param name="unit"></param>
    public void UpdateHeroBar(BattleHero unit)
    {
        Slider healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        Slider skillBar = GameObject.Find("SkillBar").GetComponent<Slider>();
        if (healthBar != null)
        {
            healthBar.value = unit.GetHealthValue();
        }
        if (skillBar != null)
        {
            skillBar.value = unit.GetSkillValue();
        }
    }

    public void UpdateEnemyInfo(BattleEnemy enemy)
    {
        if(enemy == null)
        {
            Debug.Log("null enemy.");
            return;
        }
        //avatar
        Transform enemyAvatarCamera = GameObject.Find("EnemyCamera").transform;
        enemyAvatarCamera.position = new Vector3(enemy.GetPosition(), enemy.transform.position.y, enemyAvatarCamera.position.z);
        enemyAvatarCamera.SetParent(enemy.transform);

        //name
        Text enemyName = GameObject.Find("EnemyName").GetComponent<Text>();
        enemyName.text = enemy.name;


        //health bar and skill bat
        UpdateEnemyBar(enemy);

    }

    /// <summary>
    /// //health bar and skill bat
    /// </summary>
    /// <param name="enemy"></param>
    public void UpdateEnemyBar(BattleEnemy enemy)
    {
        Slider healthBar = GameObject.Find("EnemyHealthBar").GetComponent<Slider>();
        Slider skillBar = GameObject.Find("EnemySkillBar").GetComponent<Slider>();
        if (healthBar != null)
        {
            healthBar.value = enemy.GetHealthValue();
            healthBar.interactable = false;
        }
        if (skillBar != null)
        {
            skillBar.value = enemy.GetSkillValue();
            skillBar.interactable = false;
        }
    }
    public void ResetChosenEnemy()
    {
        enemyTarget = null;
        DeactivateEnemyInfo();
    }

    public void ResetEnemyCamera()
    {
        Transform enemyAvatarCamera = GameObject.Find("EnemyCamera").transform;
        enemyAvatarCamera.SetParent(GameObject.Find("UIManager").transform);
    }

    public void ResetHeroCamera()
    {
        Transform avatarCamera = GameObject.Find("HeroCamera").transform;
        avatarCamera.SetParent(GameObject.Find("UIManager").transform);
    }
}
