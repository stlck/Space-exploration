using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;

public class WalkingEnemy : NpcBase{

    public AIPath NavAILerp;
    public Seeker Seeker;
    public SimpleSmoothModifier Modifier;

    float targetRefreshTimer = 0f;

    public override void Start()
    {
        base.Start();
        currentTime = 1f;

        if(MyAvatar.Instance.isServer)
        {
            Seeker.enabled = true;
            NavAILerp.enabled = true;
            Modifier.enabled = true;
        }
        else
        {
            Seeker.enabled = false;
            NavAILerp.enabled = false;
            Modifier.enabled = false;
        }
    }

    void stopNavigating()
    {
        NavAILerp.target = null;
        NavAILerp.canMove = false;
    }

    void startNavigating()
    {
        if(CurrentTarget != null)
        {
            NavAILerp.canMove = true;
            NavAILerp.target = CurrentTarget;
        }
    }

    public override void Update()
    {
        if(MyAvatar.Instance.isServer)
        {
            targetRefreshTimer += Time.deltaTime;
            RaycastHit hit;
            if (CurrentTarget != null && CurrentTarget.gameObject.activeInHierarchy && Physics.Raycast(transform.position + transform.forward, CurrentTarget.position - transform.position, out hit, AttackRange) && hit.transform == CurrentTarget)
            {
                stopNavigating();
                var lookAtPosition = CurrentTarget.position;
                lookAtPosition.y = transform.position.y;
                transform.LookAt(CurrentTarget);
                if (Weapon != null && Weapon.CanFire())
                {
                    Weapon.FireWeapon();
                }
            }
            else if (targetRefreshTimer >= .5f)
            {
                // it timer - find target
                if (NetworkHelper.Instance.AllPlayers.Any(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange && m.CurrentState != States.Dead))
                {
                    var temp = NetworkHelper.Instance.AllPlayers.First(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange).transform;
                    if (Vector3.Distance(temp.position, transform.position) > AttackRange)
                    {
                        CurrentTarget = temp;
                        startNavigating();
                    }
                    else
                        stopNavigating();

                }
                targetRefreshTimer = 0f;
            }

            if (CurrentTarget != null && Vector3.Distance(CurrentTarget.position, transform.position) < AttackRange)
            {
                stopNavigating();
            }
            
            // if can see && within attackrange - attack
            /*else if (Route.Any())
            {
                currentTimer += Time.deltaTime * WalkingSpeed;
                var tpos = Spawner.transform.TransformPoint(Route[0]);
                tpos.y = transform.position.y;
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
            }*/
        }
    }

    public override void Attack (Transform target)
    {
        //base.Attack(target);

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
