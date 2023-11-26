using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Control the action of enemies in the battle
public class BattleEnemy : MonoBehaviour
{
    private Animator m_Animator;
    //bool check if unit action is over.
    private bool m_actionDone = false;
    protected virtual void Awake()
    {
        //Register to the battle manager.
        BattleManager.instance.AddEnemy(this);

        m_actionDone = false;
    }

    /// <summary>
    /// Take the damage by the enemy.
    /// </summary>
    /// <typeparam name="T">Enemy script type</typeparam>
    /// <param name="Attacker">Attacker</param>
    protected virtual void TakeDamage<T>(T Attacker) where T : BattleHero
    {

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

    public void StartTimer()
    {
        StartCoroutine (Timer());
    }

    public virtual void ResetActionDone()
    {
        m_actionDone = false;
    }
}
