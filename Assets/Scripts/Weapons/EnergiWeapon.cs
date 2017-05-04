using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergiWeapon : BaseWeapon {

    public BoolUpdate IsOn;
    public List<LaserEffectObject> LaserEffectObjects;
    //public float Range = 15;
    public float LightUpTime = .25f;

    public Transform HitEffectObject;
    public ParticleSystem HitParticles;

    float ttl = 0f;

    public override void Start ()
    {
        base.Start();

        IsOn.Invoke(false);
    }

    public override void FireWeapon ()
    {
        base.FireWeapon();

        IsOn.Invoke(true);
        ttl = LightUpTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward , out hit, WeaponValues.Range))
        {
            LaserEffectObjects.ForEach(m => m.MaxLength = hit.distance);
            var s = hit.transform.GetComponent<StatBase>();
            if (s != null)
                s.TakeDamage(WeaponValues.Damage, hit.point, transform.forward);
            else
            {
                var block = hit.transform.GetComponent<Duplicate>();
                if (block != null)
                    block.ApplyDamage(hit.point, WeaponValues.Damage);
            }

            if (HitEffectObject != null)
            {
                HitEffectObject.position = hit.point;
                HitEffectObject.gameObject.SetActive(true);
            }
        }
        else
        {
            LaserEffectObjects.ForEach(m => m.MaxLength = WeaponValues.Range);
            //if(HitParticles != null)
            //    HitParticles.Pause();

            if (HitEffectObject != null)
                HitEffectObject.gameObject.SetActive(false);
        }
    }

    public override void Update ()
    {
        base.Update();
        if (ttl > 0)
        {
            ttl -= Time.deltaTime;
            if (ttl <= 0f)
            { 
                IsOn.Invoke(false);
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, WeaponValues.Range))
                {
                    LaserEffectObjects.ForEach(m => m.MaxLength = hit.distance);
                    if (HitParticles != null)
                    {
                        HitParticles.transform.position = hit.point;
                        HitParticles.Emit(1);
                    }

                }
            }

        }
    }

    public override void ShopGUI ()
    {
        base.ShopGUI();

        GUILayout.Label("Stream");

    }

   /* [System.Serializable]
    public struct EnergyWeaponValues
    {

    }*/
}
