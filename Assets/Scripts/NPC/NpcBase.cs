using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NpcBase : BaseAddForceObject {

    public NetworkIdentity NetworkIdentity;

    public float AttackRange = 10;
    public float ChaseRange = 20;

    public Transform CurrentTarget;

    public float currentTimer = 0f;
    public float currentTime = 0f;

    public InstantiatedLocation Spawner;
    public BaseWeapon Weapon;
    public Transform WeaponPoint;

    public virtual void SpawnEnemy(InstantiatedLocation owner, Vector3 position)
    {
        NetworkServer.Spawn(gameObject);

        Spawner = owner;
        transform.position = position;
        
    }

    // Use this for initialization
    public override void Start()
    {
    }

    // Update is called once per frame
    public virtual void Update()
    {
        currentTimer += Time.deltaTime;
    }

    // removed states for the time being
    public virtual void Chase(Vector3 position)
    {

    }

    public virtual void Attack(Transform target)
    {

    }

    public virtual void Idle()
    {

    }

}

public enum NPCStates
{
    Idle,
    Chase,
    Attacking
}
