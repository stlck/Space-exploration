using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAttackState : NpcBaseState
{
    public Transform AttackTarget;

    public void StartState()
    {
    }

    public void UpdateState()
    {
        //  if out of range chase
        // else attack
    }
}
