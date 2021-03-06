﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    public BaseWeapon Owner;
    public GameObject HitEffect;
    public float MoveSpeed = 10;
    public float HitRadius = 3;
    public float ExplosionForce = 50;
    Rigidbody rb;
    bool hasHit = false;

    public ColorUpdate WeaponColor;
    public List<ParticleSystem> ParticleSystems;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        if( rb.velocity != transform.forward * MoveSpeed)
            rb.AddForce(transform.forward * MoveSpeed, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit)
        {
            Destroy(gameObject);
            return;
        }

        hasHit = true;

        var tStats = collision.transform.GetComponent<StatBase>();
        if (tStats != null)
        {
            tStats.TakeDamage(Owner.WeaponValues.Damage, collision.contacts[0].point, transform.forward);
        }

        var point = collision.contacts[0].point;
        var duplicates = Physics.OverlapSphere(point, HitRadius);
        //Debug.Log("Found " + duplicates.Length);
        if(duplicates.Length == 0 && collision.gameObject.GetComponent<BaseAddForceObject>() != null)
            collision.gameObject.GetComponent<BaseAddForceObject>().ApplyForce(point, ExplosionForce, HitRadius, Owner.WeaponValues.Damage);
        else
            foreach (var d in duplicates)
            {
                if (d.gameObject != collision.gameObject && d.GetComponent<BaseAddForceObject>() != null)
                {
                    d.GetComponent<BaseAddForceObject>().ApplyForce(point, ExplosionForce, HitRadius, Owner.WeaponValues.Damage);
                }
            }

        if (HitEffect != null)
        {
            //var hit = Instantiate(HitEffect, collision.contacts[0].point, HitEffect.transform.rotation);
            HitEffect.transform.SetParent(null);
            HitEffect.transform.localScale = Vector3.one;
            HitEffect.transform.position = collision.contacts[0].point;
            HitEffect.SetActive(true);
            Destroy(HitEffect, 10f);
        }

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        Destroy(gameObject,2f);
        this.enabled = false;
    }    
}
