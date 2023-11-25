using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BattleState
{
    public void OnStateEnter();
    public void OnStateLeave();
    public void OnStateAction();
}
