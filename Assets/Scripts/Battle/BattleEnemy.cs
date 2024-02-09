using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Control the action of enemies in the battle
public class BattleEnemy : MonoBehaviour
{
    protected Animator m_Animator;
    //bool check if unit action is over.
    protected bool m_actionDone = false;
    protected int m_hp;
    protected int m_maxhp;
    protected int m_sp;
    protected int m_maxsp;
    protected int m_attackValue;
    protected float m_OriginPos;
    protected bool m_MoveToTargetDone;
    protected bool m_AttackStart;
    protected bool m_AttackEnd;
    protected SpriteRenderer m_SpriteRenderer;
    protected float m_MoveSpeed = 6.0f;

    protected virtual void Awake()
    {
        //Register to the battle manager.
        BattleManager.instance.RegisterEnemy(this);

        ResetBool();

        m_Animator = GetComponent<Animator>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_OriginPos = GetPosition();
    }

    /// <summary>
    /// Take the damage by the enemy.
    /// </summary>
    /// <typeparam name="T">Enemy script type</typeparam>
    /// <param name="Attacker">Attacker</param>
    public virtual void TakeDamage<T>(T Attacker) where T : BattleHero
    {
        //Animation
        m_Animator.CrossFade("TakeDamage", 0f);
        //hp
        m_hp -= Attacker.GetAttackValue();
        //UI
        BattleUIManager.instance.UpdateEnemyBar(this);
        OnEnemyDeath();
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
        if (Mathf.Abs(GetPosition() - destination) < 0.1f)
        {
            return true;
        }
        //移动
        //当前位置在目标位置右边，向左移动
        if ((destination - GetPosition() <= 0))
        {
            m_SpriteRenderer.flipX = true;
            transform.Translate(Vector3.left * Time.deltaTime * m_MoveSpeed);
        }
        else
        {
            m_SpriteRenderer.flipX=false;
            transform.Translate(Vector3.right * Time.deltaTime * m_MoveSpeed);
        }
        return false;
    }

    public virtual bool Move(float destination) 
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

    public virtual bool Attack()
    {
        //随机获取一个目标
        BattleHero target = BattleManager.instance.RandomAliveHero();

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
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == false)
            {
                m_Animator.CrossFade("Attack1", 0f);
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
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
            {
                m_Animator.CrossFade("Idle", 0f);
            }
            m_SpriteRenderer.flipX = true;
        }
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
            BattleUIManager.instance.ResetEnemyCamera();
            GetComponent<BoxCollider2D>().enabled = false;
            
        }
    }

    public void StartTimer()
    {
        StartCoroutine (Timer());
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
