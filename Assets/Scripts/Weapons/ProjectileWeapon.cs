using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : BaseWeapon {

    public BaseProjectile Projectile;

    public override void FireWeapon()
    {
        base.FireWeapon();

        var t = Instantiate(Projectile, transform.position + Vector3.forward, transform.rotation);
        t.Owner = this;
    }

}
