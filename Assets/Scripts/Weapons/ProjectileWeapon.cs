using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : BaseWeapon {

    public BaseProjectile Projectile;

    public override void FireWeapon()
    {
        base.FireWeapon();

        var t = Instantiate(Projectile, transform.position + transform.forward, transform.rotation);
        if (!t.gameObject.activeInHierarchy)
            t.gameObject.SetActive(true);
        t.WeaponColor.Invoke(WeaponValues.WeaponColor);

        t.ParticleSystems.ForEach(m =>
        {
            var main = m.main;
            main.startColor = WeaponValues.WeaponColor;
        });

        t.Owner = this;
    }
    public override void ShopGUI ()
    {
        base.ShopGUI();

        GUILayout.Label("Projectile");
    }

}
