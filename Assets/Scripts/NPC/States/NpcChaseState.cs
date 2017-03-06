using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcChaseState : NpcBaseState
{
    public Transform ChaseTarget;

    public void StartState()
    {
    }

    public void UpdateState()
    {
        // if target not null, move close
        // if close enough, go attack
    }
}
