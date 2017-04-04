using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WalkingEnemy : NpcBase{

    public BaseWeapon Weapon;
    public List<Vector3> Route;
    Vector3 currentRouteTarget;
    Vector3 prevPosition;
    BestFirstSearch bfs;

    public float WalkingSpeed = 5f;
    float targetRefreshTimer = 0f;
    public override void Start()
    {
        base.Start();
        currentTime = 1f;
        if(Spawner.TargetLocation.BestFirstSearch != null)
        {
            bfs = Spawner.TargetLocation.BestFirstSearch;

            if (CurrentTarget != null)
                Route = bfs.FindPath((int)transform.position.x, (int)transform.position.z, (int)CurrentTarget.position.x, (int)CurrentTarget.position.z, 1);
        }
    }


    public override void Update()
    {
        //base.Update();
        //currentTimer += Time.deltaTime;
        targetRefreshTimer += Time.deltaTime;
        if (targetRefreshTimer >= 1f)
        {
            // it timer - find target
            if (NetworkHelper.Instance.AllPlayers.Any(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange))
            {
                CurrentTarget = NetworkHelper.Instance.AllPlayers.First(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange).transform;
                var tPos = ownerLocalPosition(CurrentTarget.position);
                var lPos = ownerLocalPosition(transform.position);
                Route = bfs.FindPath((int)transform.localPosition.x, (int)transform.localPosition.z, (int)CurrentTarget.localPosition.x, (int)CurrentTarget.localPosition.z, 1);
                //Route = bfs.FindPath((int)transform.localPosition.x, (int)transform.localPosition.z, (int)CurrentTarget.localPosition.x, (int)CurrentTarget.localPosition.z, 1);
            }
            targetRefreshTimer = 0f;
        }
        RaycastHit hit;
        if(CurrentTarget != null && Physics.Raycast(transform.position + transform.forward, CurrentTarget.position - transform.position, out hit, AttackRange) && hit.transform == CurrentTarget)
        {
            transform.LookAt(CurrentTarget);
            if (Weapon != null && Weapon.CanFire())
                Weapon.FireWeapon();
        }
        // if can see && within attackrange - attack
        //  else
        else if (Route.Any())
        {
            currentTimer += Time.deltaTime * WalkingSpeed;
            var tpos = Spawner.transform.TransformPoint(Route[0]);
            transform.position = Vector3.MoveTowards(transform.position, tpos, Time.deltaTime * WalkingSpeed);
            transform.LookAt(tpos);
            //transform.position = Vector3.Lerp(prevPosition, tpos, currentTimer / currentTime);

            if ( Vector3.Distance( transform.position, tpos) < 1f)
            //if (currentTimer >= currentTime)
            {
                var tPos = ownerLocalPosition(CurrentTarget.position);
                var lPos = ownerLocalPosition(transform.position);

                //Route = bfs.FindPath((int)transform.localPosition.x, (int)transform.localPosition.z, (int)CurrentTarget.localPosition.x, (int)CurrentTarget.localPosition.z, 1);
                //if (Route.Any())
                Route.RemoveAt(0);
                currentTimer = 0f;
            }
        }

        if(CurrentTarget != null && Vector3.Distance(CurrentTarget.position, transform.position) > ChaseRange)
        {
            Route.Clear();
            CurrentTarget = null;
        }
    }

    Vector3 ownerLocalPosition(Vector3 position)
    {
        return Spawner.transform.InverseTransformVector(position);
    }

    Vector3 ownerWorldPosition(Vector3 localPosition)
    {
        return Spawner.transform.TransformVector(localPosition);
    }

    public override void Attack(Transform target)
    {
        base.Attack(target);
        if (Weapon != null && Weapon.CanFire())
            Weapon.FireWeapon();
    }

    public override void UpdateAttack ()
    {
        base.UpdateAttack();
    }

    public override void SetAttackState (Transform target)
    {
        Ray r = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(r, AttackRange, LayerMask.NameToLayer("Player")))
            base.SetAttackState(target);
    }
}
