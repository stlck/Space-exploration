using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : NpcBase{

    public BaseWeapon TurretWeapon;

    public override void Update()
    {
        base.Update();

    }

    public override void UpdateChase()
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
    }
}
