using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


//Control the action of players in the battle
public class BattleHero : MonoBehaviour
{
    private Animator m_Animator;

    private float m_hp;
    private float m_maxhp;

    

    protected virtual void Awake()
    {
        //Register to the battle manager
        BattleManager.instance.AddTeamMembers(this);
        m_Animator = GetComponent<Animator>();
        m_maxhp = 100f;
        m_hp = m_maxhp;
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
}