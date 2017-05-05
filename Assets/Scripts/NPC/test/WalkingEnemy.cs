using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WalkingEnemy : NpcBase{

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
        if(MyAvatar.Instance.isServer)
        { 
            targetRefreshTimer += Time.deltaTime;
            if (targetRefreshTimer >= .5f)
            {
                // it timer - find target
                if (NetworkHelper.Instance.AllPlayers.Any(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange && m.CurrentState != States.Dead))
                {
                    CurrentTarget = NetworkHelper.Instance.AllPlayers.First(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange).transform;
                    var tPos = ownerLocalPosition(CurrentTarget.position);
                    var lPos = ownerLocalPosition(transform.position);
                    Route = bfs.FindPath((int)transform.localPosition.x, (int)transform.localPosition.z, (int)CurrentTarget.localPosition.x, (int)CurrentTarget.localPosition.z, 1);
                }
                targetRefreshTimer = 0f;
            }
            RaycastHit hit;
            if(CurrentTarget != null && CurrentTarget.gameObject.activeInHierarchy && Physics.Raycast(transform.position + transform.forward, CurrentTarget.position - transform.position, out hit, AttackRange) && hit.transform == CurrentTarget)
            {
                var lookAtPosition = CurrentTarget.position;
                lookAtPosition.y = transform.position.y;
                transform.LookAt(CurrentTarget);
                if (Weapon != null && Weapon.CanFire())
                    Weapon.FireWeapon();
            }
            // if can see && within attackrange - attack
            else if (Route.Any())
            {
                currentTimer += Time.deltaTime * WalkingSpeed;
                var tpos = Spawner.transform.TransformPoint(Route[0]);
                transform.position = Vector3.MoveTowards(transform.position, tpos, Time.deltaTime * WalkingSpeed);
                transform.LookAt(tpos);

                if ( Vector3.Distance( transform.position, tpos) < 1f)
                {
                    var tPos = ownerLocalPosition(CurrentTarget.position);
                    var lPos = ownerLocalPosition(transform.position);

                    Route.RemoveAt(0);
                    currentTimer = 0f;
                }
            }

            if(CurrentTarget != null && (Vector3.Distance(CurrentTarget.position, transform.position) > ChaseRange || !CurrentTarget.gameObject.activeInHierarchy))
            {
                Route.Clear();
                CurrentTarget = null;
            }
        }
    }

    public override void Attack (Transform target)
    {
        base.Attack(target);

    }

    Vector3 ownerLocalPosition(Vector3 position)
    {
        return Spawner.transform.InverseTransformVector(position);
    }

    Vector3 ownerWorldPosition(Vector3 localPosition)
    {
        return Spawner.transform.TransformVector(localPosition);
    }
}
