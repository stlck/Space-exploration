using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergiWeapon : BaseWeapon {

    public BoolUpdate IsOn;
    public List<LaserEffectObject> LaserEffectObjects;
    public float Range = 15;
    public float LightUpTime = .25f;

    public Transform HitEffectObject;

    //public EnergyWeaponValues EnergyValues;

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
        if (Physics.Raycast(transform.position, transform.forward , out hit, Range))
        {
            LaserEffectObjects.ForEach(m => m.MaxLength = hit.distance);
            var s = hit.  transform.GetComponent<StatBase>();
            if (s != null)
                s.TakeDamage(DamagePerHit, hit.point, transform.forward);

            if(HitEffectObject != null)
            {
                HitEffectObject.position = hit.point;
                HitEffectObject.gameObject.SetActive(true);
            }
        }
        else
        {
            LaserEffectObjects.ForEach(m => m.MaxLength = Range);
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
                IsOn.Invoke(false);
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
