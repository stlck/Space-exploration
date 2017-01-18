using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamWeapon : BaseWeapon {

    public ParticleSystem VisualEffects;
    
    public override void FireWeapon()
    {
        base.FireWeapon();

        VisualEffects.Play();

        
        var hits = Physics.SphereCastAll(transform.position + transform.forward * 1.8f, 2, transform.forward);
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
