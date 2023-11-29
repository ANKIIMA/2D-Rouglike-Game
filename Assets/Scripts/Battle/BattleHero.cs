using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//Control the action of players in the battle
public class BattleHero : MonoBehaviour
{
    private Animator m_Animator;

    private int m_hp;
    private int m_maxhp;

    private int m_sp;
    private int m_maxsp;

    private bool m_actionDone = false;

    protected virtual void Awake()
    {
        //Register to the battle manager
        BattleManager.instance.AddTeamMembers(this);
        m_Animator = GetComponent<Animator>();
        //initiate hp and sp
        m_maxhp = 100;
        m_hp = m_maxhp;
        m_maxsp = 100;
        m_sp = m_maxsp;
        //reset actionDone
        m_actionDone = false;
    }

    /// <summary>
    /// Take the damage by the enemy.
    /// </summary>
    /// <typeparam name="T">Enemy script type</typeparam>
    /// <param name="Attacker">Attacker</param>
    protected virtual void TakeDamage<T>(T Attacker) where T : BattleEnemy
    {

    }

    /// <summary>
    /// Attack the attacked.
    /// </summary>
    /// <typeparam name="T">Enemy script type</typeparam>
    /// <param name="theAttacked">the Attacked one</param>
    protected virtual void Attack<T>(T theAttacked) where T : BattleEnemy
    {

    }

    /// <summary>
    /// return the x position of the unit.
    /// </summary>
    /// <returns>x coordinate of unit</returns>
    public float GetPosition()
    {
        return gameObject.transform.position.x;
    }

    /// <summary>
    /// Move toward the target.
    /// </summary>
    /// <typeparam name="T">Enemy Script type</typeparam>
    /// <param name="target">target script</param>
    /// <returns>is moving completed?</returns>
    protected virtual bool Move<T> (T target) where T : BattleEnemy
    {
        float destination = target.GetPosition();
        //已经移动到指定位置
        if (Mathf.Abs(GetPosition() - destination) < 0.2f)
        {
            Debug.Log("Reach the destination.");
            return true; 
        }
        //向右移动的距离
        transform.Translate(new Vector3(destination * Time.deltaTime, 0, 0));
        return false;
    }

    protected virtual bool Attack1()
    {


        //已经移动到敌人了
        //m_Animator.CrossFade("Attack1", 0.1f);

        //2s后返回true.

        return m_actionDone;
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(2);

        m_actionDone = true;

        yield break;
    }

    public void StartTimer()
    {
        StartCoroutine(Timer());
    }

    protected virtual bool Attack2()
    {
        return false;
    }

    protected virtual bool Skill1()
    {
        return false;
    }

    protected virtual bool Skill2()
    {
        return false;
    }

    /// <summary>
    /// act according the index and target.
    /// </summary>
    /// <param name="index">action index</param>
    /// <param name="target">target enemy</param>
    /// <returns></returns>
    public virtual bool Action(int index, BattleEnemy target)
    {
        switch (index)
        {
            case 0: return Attack1();
            case 1: return Attack2();
            case 2: return Skill1();
            case 3: return Skill2();
            default: return false;
        }
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
}