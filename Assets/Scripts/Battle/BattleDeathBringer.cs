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
        //�Ѿ��ƶ���ָ��λ��
        if (Mathf.Abs(GetPosition() - destination) < 0.1f)
        {
            return true;
        }
        //�ƶ�
        //��ǰλ����Ŀ��λ���ұߣ������ƶ�
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
        //�Ѿ��ƶ���ָ��λ��
        if (Mathf.Abs(GetPosition() - destination) < 0.1f)
        {
            return true;
        }
        //�ƶ�
        //��ǰλ����Ŀ��λ���ұߣ������ƶ�
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
        //�����ȡһ��Ŀ��
        BattleHero target = BattleManager.instance.RandomAliveHero();

        //��δ�ƶ���ָ��λ��
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
        //�ƶ����˵���û��ʼ����
        else if (m_MoveToTargetDone == true && m_AttackStart == false)
        {
            m_AttackStart = true;
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == false)
            {
                m_Animator.CrossFade("Attack1", 0f);
            }
            target.TakeDamage(this);
        }
        //������ʼ�˵�û����
        else if (m_AttackStart == true && m_AttackEnd == false)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
            {
                m_AttackEnd = true;
            }
        }
        //����������
        else if (m_AttackEnd == true)
        {
            if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Run") == false)
            {
                m_Animator.CrossFade("Run", 0f);
            }
            m_actionDone = Move(m_OriginPos);
        }
        //�ص���ԭ��
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
