using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretEnemy : NpcBase{

    public BaseWeapon TurretWeapon;
    float targetRefreshTimer = 0f;
    public Transform RotateYPart;
    public Transform WeaponPoint;

    public override void Start ()
    {
        base.Start();
        if (!TurretWeapon.gameObject.activeInHierarchy)
        {
            TurretWeapon = Instantiate(TurretWeapon, WeaponPoint);
            TurretWeapon.transform.localScale = Vector3.one;
        }
    }

    public override void Update()
    {
        base.Update();

        targetRefreshTimer += Time.deltaTime;
        if (targetRefreshTimer >= 1f)
        {
            // it timer - find target
            if (NetworkHelper.Instance.AllPlayers.Any(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange))
            {
                CurrentTarget = NetworkHelper.Instance.AllPlayers.First(m => Vector3.Distance(m.transform.position, transform.position) < ChaseRange).transform;
                //var tPos = ownerLocalPosition(CurrentTarget.position);
                //var lPos = ownerLocalPosition(transform.position);
                //Route = bfs.FindPath((int)transform.localPosition.x, (int)transform.localPosition.z, (int)CurrentTarget.localPosition.x, (int)CurrentTarget.localPosition.z, 1);
            }
            targetRefreshTimer = 0f;
        }

        if(CurrentTarget != null)
        {
            var yLookAt = CurrentTarget.position;
            yLookAt.y = RotateYPart.position.y;
            RotateYPart.LookAt(yLookAt);
            WeaponPoint.LookAt(CurrentTarget.position);

            if (TurretWeapon != null && TurretWeapon.CanFire() && Vector3.Distance(transform.position, CurrentTarget.position) < AttackRange)
            {
                if (MyAvatar.Instance.isServer)
                {
                    RaycastHit hitinfo;
                    if (Physics.Raycast(WeaponPoint.position, WeaponPoint.forward, out hitinfo, AttackRange) && hitinfo.collider.tag != "Block")
                    {
                        currentTimer = 0f;
                        TurretWeapon.FireWeapon();
                    }
                }
            }
        }
    }

   /* public override void UpdateChase()
    {
        base.UpdateChase();
        transform.LookAt(CurrentTarget.position);
    }

    public override void Chase(Vector3 position)
    {
        base.Chase(position);
        transform.LookAt(position);
    }

    public override void Attack(Transform target)
    {
        base.Attack(target);

        if (TurretWeapon.CanFire())
            TurretWeapon.FireWeapon();
    }*/
}
