using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : BaseWeapon {

    public BaseProjectile Projectile;

    //public ProjectileWeaponValues ProjectileValues;

    public override void FireWeapon()
    {
        base.FireWeapon();

        var t = Instantiate(Projectile, transform.position + transform.forward, transform.rotation);
        //Debug.Log("bullet at " + t.transform.position);  
        t.Owner = this;
    }
    public override void ShopGUI ()
    {
        base.ShopGUI();

        GUILayout.Label("Projectile");
    }

   /* [System.Serializable]
    public struct ProjectileWeaponValues
    {
        //public float Range;
        //public float Width;
    }*/
}
