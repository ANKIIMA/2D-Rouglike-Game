using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class BattleUIManager : MonoBehaviour
{
    public void OnAttack1()
    {
        Debug.Log("Attack1");
    }

    public void OnAttack2()
    {
        Debug.Log("Attack2");
    }

    public void OnPass()
    {
        Debug.Log("Pass");
    }
}
