using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcBase : BaseAddForceObject {

    public NPCStates CurrentState;
    public float ChaseTime = 1f;
    public float AttackTime = 1f;

    public float AttackRange = 10;
    public float ChaseRange = 20;

    public Transform CurrentTarget;

    public float currentTimer = 0f;
    public float currentTime = 0f;

    public Location Spawner;

    // Use this for initialization
    public override void Start()
    {
        SetIdleState();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        currentTimer += Time.deltaTime;
        UpdateState();

        if (currentTimer >= currentTime)
        {
            if (CurrentTarget == null)
                SetIdleState();
            else if (Vector3.Distance(CurrentTarget.position, transform.position) < AttackRange)
            {
                SetAttackState(CurrentTarget);
            }
            else
                SetChaseState(CurrentTarget);
        }
    }

    void UpdateState()
    {
        switch(CurrentState)
        {
            case NPCStates.Attacking:
                UpdateAttack();
                break;
            case NPCStates.Idle:
                UpdateIdle();
                break;
            case NPCStates.Moving:
                UpdateMove();
                break;
        }
    }

    public virtual void UpdateMove()
    {

    }

    public virtual void UpdateAttack()
    {

    }

    public virtual void UpdateIdle()
    {
        if(NetworkHelper.Instance.AllPlayers.Any(m => Vector3.Distance(m.transform.position,transform.position) < ChaseRange))
        {
            SetChaseState(NetworkHelper.Instance.AllPlayers.Where(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange).First().transform);
        }
    }

    public virtual void Move(Vector3 position)
    {

    }

    public virtual void Attack(Transform target)
    {

    }

    public virtual void Idle()
    {

    }

    public virtual void SetAttackState(Transform target)
    {
        currentTime = AttackTime;
        currentTimer = 0f;
        CurrentTarget = target;
        CurrentState = NPCStates.Attacking;
        Attack(target);
    }

    public virtual void SetChaseState(Transform target)
    {
        currentTime = ChaseTime;
        currentTimer = 0f;
        CurrentTarget = target;
        CurrentState = NPCStates.Moving;
        Move(target.position);
    }

    public virtual void SetIdleState()
    {
        CurrentState = NPCStates.Idle;
        currentTime = 1f;
        currentTimer = 0f;
    }
}

public enum NPCStates
{
    Idle,
    Moving,
    Attacking
}
