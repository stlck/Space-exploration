using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamWeapon : BaseWeapon {

    public ParticleSystem VisualEffects;
    public float Range = 15;
    public float Width = 2;
    public override void FireWeapon()
    {
        base.FireWeapon();

        VisualEffects.Play();

        
        var hits = Physics.SphereCastAll(transform.position + transform.forward * Width * 1.1f, Width, transform.forward, Range);
        foreach (var h in hits)
        {
            var s = h.transform.GetComponent<StatBase>();
            if (s != null)
                s.TakeDamage(DamagePerHit);
        }
    }

    public override void Update()
    {
        base.Update();

    }
}
