using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamWeapon : BaseWeapon {

    public ParticleSystem VisualEffects;
    //public float Range = 15;
    public float Width = 2;
    public BoolUpdate IsOn;
    float ttl = 0f;
    public float LightUpTime = .25f;

    //public StreamWeaponValues StreamValues;

    public override void FireWeapon()
    {
        base.FireWeapon();

        VisualEffects.Play();

        IsOn .Invoke( true);
        ttl = LightUpTime;

        var hits = Physics.SphereCastAll(transform.position + transform.forward * Width * 1.1f, Width, transform.forward, WeaponValues.Range);
        foreach (var h in hits)
        {
            var s = h.transform.GetComponent<StatBase>();
            if (s != null)
                s.TakeDamage(WeaponValues.Damage, h.point, transform.forward);
        }
    }

    public override void Update()
    {
        base.Update();
        if( ttl > 0)
        {
            ttl -= Time.deltaTime;
            if (ttl <= 0f)
                IsOn .Invoke( false);
        }
    }

    public override void ShopGUI ()
    {
        base.ShopGUI();

        GUILayout.Label("Stream");

    }

   /* [System.Serializable]
    public struct StreamWeaponValues
    {
        public float Width;
    }*/
}
