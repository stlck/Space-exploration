using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    public BaseWeapon Owner;
    public GameObject HitEffect;

    public void OnCollisionEnter(Collision collision)
    {
        if(MyAvatar.Instance.isServer)
        { 
            var tStats = collision.transform.GetComponent<StatBase>();
            if (tStats != null)
            {
                tStats.TakeDamage(Owner.DamagePerHit);
            }
        }

        if (HitEffect != null)
        {
            Destroy(Instantiate(HitEffect, collision.contacts[0].point, HitEffect.transform.rotation), 10f);
        }
        Destroy(gameObject);
    }    
}
