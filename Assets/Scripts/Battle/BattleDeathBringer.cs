using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDeathBringer : BattleEnemy
{
    protected override void Awake()
    {
        base.Awake();

        LoadData();
    }

    private void LoadData()
    {
        DeathBringerSO data = (DeathBringerSO)Resources.Load("SOAssets/DeathBringerData");

        m_maxhp = data.m_maxhp;
        m_hp = m_maxhp;

        m_maxsp = data.m_maxsp;
        m_sp = m_maxsp;

        m_attackValue = data.m_attackValue;

    }

    public override bool Move<T>(T target)
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
            m_SpriteRenderer.flipX = false;
            transform.Translate(Vector3.left * Time.deltaTime * m_MoveSpeed);
        }
        else
        {
            m_SpriteRenderer.flipX = true;
            transform.Translate(Vector3.right * Time.deltaTime * m_MoveSpeed);
        }
        return false;
    }

    public override bool Move(float destination)
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
            m_SpriteRenderer.flipX = false;
            transform.Translate(Vector3.left * Time.deltaTime * m_MoveSpeed);
        }
        else
        {
            m_SpriteRenderer.flipX = true;
            transform.Translate(Vector3.right * Time.deltaTime * m_MoveSpeed);
        }
        return false;
    }

    public override bool Attack()
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
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
            {
                m_Animator.CrossFade("Idle", 0f);
            }
            m_SpriteRenderer.flipX = false;
        }
        return m_actionDone;
    }

}
