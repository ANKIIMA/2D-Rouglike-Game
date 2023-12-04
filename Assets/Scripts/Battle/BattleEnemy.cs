using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Control the action of enemies in the battle
public class BattleEnemy : MonoBehaviour
{
    private Animator m_Animator;
    //bool check if unit action is over.
    private bool m_actionDone = false;
    private int m_hp;
    private int m_maxhp;
    private int m_sp;
    private int m_maxsp;
    private int m_attackValue;
    protected virtual void Awake()
    {
        //Register to the battle manager.
        BattleManager.instance.RegisterEnemy(this);

        m_actionDone = false;

        m_maxhp = 100;
        m_hp = m_maxhp;
        m_maxsp = 100;
        m_sp = m_maxsp;
        m_attackValue = 10;

        m_Animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Take the damage by the enemy.
    /// </summary>
    /// <typeparam name="T">Enemy script type</typeparam>
    /// <param name="Attacker">Attacker</param>
    public virtual void TakeDamage<T>(T Attacker) where T : BattleHero
    {
        //hp
        m_hp -= Attacker.GetAttackValue();
        //UI
        BattleUIManager.instance.UpdateEnemyBar(this);
        OnEnemyDeath();
    }

    /// <summary>
    /// Attack the attacked.
    /// </summary>
    /// <typeparam name="T">Enemy script type</typeparam>
    /// <param name="theAttacked">the Attacked one</param>
    protected virtual void Attack<T>(T theAttacked) where T : BattleHero
    {

    }

    /// <summary>
    /// return the x position of the unit.
    /// </summary>
    /// <returns>x coordinate of unit</returns>
    public virtual float GetPosition()
    {
        return gameObject.transform.position.x;
    }

    /// <summary>
    /// Move toward the target.
    /// </summary>
    /// <typeparam name="T">Enemy Script type</typeparam>
    /// <param name="target">target script</param>
    /// <returns>is moving completed?</returns>
    public virtual bool Move<T>(T target) where T : BattleHero
    {
        float destination = target.GetPosition();
        //已经移动到指定位置
        if (Mathf.Abs(GetPosition() - destination) < 0.2f)
        {
            Debug.Log("Reach the destination.");
            return true;
        }
        //向左移动的距离
        transform.Translate(new Vector3(-destination * Time.deltaTime, 0, 0));
        return false;
    }

    public virtual bool Attack()
    {
        //移动，攻击，返回移动

        return m_actionDone;
    }

    //wait for 2s and return true;
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(2);

        m_actionDone = true;

        yield break;
    }

    private void OnEnemyDeath()
    {
        //Enemy died
        if(m_hp <= 0)
        {
            m_Animator.CrossFade("Death", 0f);
            BattleManager.instance.DeleteObjectInBattleQueue(this);
            BattleUIManager.instance.ResetChosenEnemy();
            GetComponent<BoxCollider2D>().enabled = false;

        }
    }

    public void StartTimer()
    {
        StartCoroutine (Timer());
    }

    public virtual void ResetActionDone()
    {
        m_actionDone = false;
    }

    public float GetHealthValue()
    {
        return (float)m_hp / m_maxhp;
    }

    public float GetSkillValue()
    {
        return (float)m_sp / m_maxsp;
    }

    public int GetAttackValue()
    {
        return m_attackValue;
    }
}
