using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NpcBase : BaseAddForceObject {

    public NetworkIdentity NetworkIdentity;

    public NPCStates CurrentState;
    public float ChaseTime = 1f;
    public float AttackTime = 1f;

    public float AttackRange = 10;
    public float ChaseRange = 20;

    public Transform CurrentTarget;

    public float currentTimer = 0f;
    public float currentTime = 0f;

    public InstantiatedLocation Spawner;

    public virtual void SpawnEnemy(InstantiatedLocation owner, Vector3 position)
    {
        NetworkServer.Spawn(gameObject);

        Spawner = owner;
        transform.position = position;
    }

    // Use this for initialization
    public override void Start()
    {
        //SetIdleState();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        currentTimer += Time.deltaTime;

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

        UpdateState();
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
            case NPCStates.Chase:
                UpdateChase();
                break;
        }
    }

    public virtual void UpdateChase()
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

    public virtual void Chase(Vector3 position)
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
        transform.LookAt(target);
        Attack(target);
    }

    public virtual void SetChaseState(Transform target)
    {
        currentTime = ChaseTime;
        currentTimer = 0f;
        CurrentTarget = target;
        CurrentState = NPCStates.Chase;
        Chase(target.position);
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
    Chase,
    Attacking
}
