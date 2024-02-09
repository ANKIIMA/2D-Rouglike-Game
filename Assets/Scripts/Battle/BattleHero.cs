using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;


//Control the action of players in the battle
public class BattleHero : MonoBehaviour
{
    private Animator m_Animator;
    private SpriteRenderer m_SpriteRenderer;

    public string m_type;

    protected int m_hp;
    protected int m_maxhp;

    protected int m_sp;
    protected int m_maxsp;

    protected int m_attackValue;

    private bool m_actionDone = false;

    private bool m_MoveToTargetDone = false;

    private bool m_AttackStart = false;

    private bool m_AttackEnd = false;

    private float m_OriginPos = 0;

    private float m_MoveSpeed = 6f;


    protected virtual void Awake()
    {
        //Register to the battle manager
        BattleManager.instance.RegisterHero(this);
        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        
        //reset actionDone
        ResetBool();
    }

    private void Start()
    {
        //记录原位置
        m_OriginPos = GetPosition();
    }

    /// <summary>
    /// Take the damage by the enemy.
    /// </summary>
    /// <typeparam name="T">Enemy script type</typeparam>
    /// <param name="Attacker">Attacker</param>
    public virtual void TakeDamage<T>(T Attacker) where T : BattleEnemy
    {
        m_Animator.CrossFade("TakeDamage", 0f);
        m_hp -= Attacker.GetAttackValue();
        OnHeroDeath();
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
        if (Mathf.Abs(GetPosition() - destination) < 0.1f)
        {
            return true; 
        }
        //移动
        //当前位置在目标位置右边，向左移动
        if((destination - GetPosition() <= 0))
        {
            //水平翻转
            m_SpriteRenderer.flipX = true;
            transform.Translate(Vector3.left * Time.deltaTime * m_MoveSpeed);
        }
        else
        {
            m_SpriteRenderer.flipX = false;
            transform.Translate(Vector3.right * Time.deltaTime * m_MoveSpeed);
        }
        return false;
    }

    protected virtual bool Move(float destination)
    {
        //已经移动到指定位置
        if (Mathf.Abs(GetPosition() - destination) < 0.1f)
        {
            return true;
        }
        //移动
        //当前位置在目标位置右边，向左移动
        if ((destination - GetPosition() <= 0))
        {
            //水平翻转
            m_SpriteRenderer.flipX = true;
            transform.Translate(Vector3.left * Time.deltaTime * m_MoveSpeed);
        }
        else
        {
            m_SpriteRenderer.flipX = false;
            transform.Translate(Vector3.right * Time.deltaTime * m_MoveSpeed);
        }
        return false;
    }

    protected virtual bool Attack1(BattleEnemy target)
    {
        //还未移动到指定位置
        if(m_MoveToTargetDone == false)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
            {
                m_Animator.CrossFade("Run", 0f);
            }
            if (Move(target) == true)
            {
                m_MoveToTargetDone = true;
            }
        }
        //移动到了但是没开始攻击
        else if(m_MoveToTargetDone == true && m_AttackStart == false) 
        {
            m_AttackStart = true;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == false)
            {
                m_Animator.CrossFade("Attack1", 0f);
            }
            target.TakeDamage(this);
        }
        //攻击开始了但没结束
        else if(m_AttackStart == true && m_AttackEnd == false)
        {
            if(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                m_AttackEnd = true;
            }
        }
        //攻击结束了
        else if(m_AttackEnd == true) 
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
            {
                m_Animator.CrossFade("Run", 0f);
            }

            m_actionDone = Move(m_OriginPos);
        }
        //回到了原点
        if(m_actionDone == true)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
            {
                m_Animator.CrossFade("Idle", 0f);
            }
            m_SpriteRenderer.flipX = false;
        }
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

    private void OnHeroDeath()
    {
        //Enemy died
        if (m_hp <= 0)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Death") == false)
            {
                m_Animator.CrossFade("Death", 0f);
            }
            BattleManager.instance.DeleteObjectInBattleQueue(this);
            BattleUIManager.instance.ResetHeroCamera();
        }
    }

    protected virtual bool Attack2(BattleEnemy target)
    {
        //还未移动到指定位置
        if (m_MoveToTargetDone == false)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
            {
                m_Animator.CrossFade("Run", 0f);
            }
            if (Move(target) == true)
            {
                m_MoveToTargetDone = true;
            }
        }
        //移动到了但是没开始攻击
        else if (m_MoveToTargetDone == true && m_AttackStart == false)
        {
            m_AttackStart = true;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") == false)
            {
                m_Animator.CrossFade("Attack2", 0f);
            }
            target.TakeDamage(this);
        }
        //攻击开始了但没结束
        else if (m_AttackStart == true && m_AttackEnd == false)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                m_AttackEnd = true;
            }
        }
        //攻击结束了
        else if (m_AttackEnd == true)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
            {
                m_Animator.CrossFade("Run", 0f);
            }

            m_actionDone = Move(m_OriginPos);
        }
        //回到了原点
        if (m_actionDone == true)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
            {
                m_Animator.CrossFade("Idle", 0f);
            }
            m_SpriteRenderer.flipX = false;
        }
        return m_actionDone;
    }

    protected virtual bool Skill1(BattleEnemy target)
    {
        return false;
    }

    protected virtual bool Skill2(BattleEnemy target)
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
            case 0: return Attack1(target);
            case 1: return Attack2(target);
            case 2: return Skill1(target);
            case 3: return Skill2(target);
            default: return false;
        }
    }

    public virtual void ResetBool()
    {
        m_actionDone = false;
        m_AttackEnd = false;
        m_AttackStart = false;
        m_MoveToTargetDone = false;
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