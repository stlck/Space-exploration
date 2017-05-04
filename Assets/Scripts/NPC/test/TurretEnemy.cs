using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurretEnemy : NpcBase{

    //public BaseWeapon TurretWeapon;
    float targetRefreshTimer = 0f;
    public Transform RotateYPart;

    public override void Start ()
    {
        base.Start();
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
            }
            targetRefreshTimer = 0f;
        }

        if(CurrentTarget != null)
        {
            var yLookAt = CurrentTarget.position;
            yLookAt.y = RotateYPart.position.y;
            RotateYPart.LookAt(yLookAt);
            WeaponPoint.LookAt(CurrentTarget.position);

            if (Weapon != null && Weapon.CanFire() && Vector3.Distance(transform.position, CurrentTarget.position) < AttackRange)
            {
                if (MyAvatar.Instance.isServer)
                {
                    RaycastHit hitinfo;
                    if (Physics.Raycast(WeaponPoint.position, WeaponPoint.forward, out hitinfo, AttackRange) && hitinfo.collider.tag != "Block")
                    {
                        currentTimer = 0f;
                        Weapon.FireWeapon();
                    }
                }
            }
        }
    }

}
