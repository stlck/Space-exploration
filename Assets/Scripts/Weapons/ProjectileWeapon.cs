using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : BaseWeapon {

    public BaseProjectile Projectile;
    public int BurstCount = 1;
    public float BurstDelay = .1f;

    private void Start ()
    {
        Projectile.WeaponColor.Invoke(WeaponValues.WeaponColor);
        Projectile.ParticleSystems.ForEach(m =>
        {
            var main = m.main;
            main.startColor = WeaponValues.WeaponColor;
        });

        Projectile.Owner = this;
    }

    public override void FireWeapon()
    {
        base.FireWeapon();

        if (BurstCount == 1)
            launchProjectile();
        else
            StartCoroutine(shootBurst());
    }
    public override void ShopGUI ()
    {
        base.ShopGUI();

        GUILayout.Label("Projectile");
    }

    IEnumerator shootBurst()
    {
        int shots = 0;
        while(shots < BurstCount)
        {
            launchProjectile();
            shots++;
            yield return new WaitForSeconds(BurstDelay);
        }
    }

    void launchProjectile()
    {
        var t = Instantiate(Projectile, transform.position + transform.forward, transform.rotation);
        if (!t.gameObject.activeInHierarchy)
            t.gameObject.SetActive(true);
        //t.WeaponColor.Invoke(WeaponValues.WeaponColor);

        //t.ParticleSystems.ForEach(m =>
        //{
        //    var main = m.main;
        //    main.startColor = WeaponValues.WeaponColor;
        //});

        //t.Owner = this;
    }
}
