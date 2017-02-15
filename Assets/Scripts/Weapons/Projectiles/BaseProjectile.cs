using System.Collections;
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
        //rb.MovePosition(transform.forward * MoveSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;
        hasHit = true;

        if ( MyAvatar.Instance.GetComponent<Collider>() == collision.collider)
        {
            Physics.IgnoreCollision(collision.collider, this.GetComponent<Collider>(), true);
            return;
        }

        //Debug.Log("Collision " + collision.gameObject.name, collision.gameObject);
        if(MyAvatar.Instance.isServer)
        { 
            //Debug.Log("Collision on server");
            var tStats = collision.transform.GetComponent<StatBase>();
            if (tStats != null)
            {
                tStats.TakeDamage(Owner.DamagePerHit);
            }
        }

        var point = collision.contacts[0].point;
        var duplicates = Physics.OverlapSphere(point, HitRadius);
        Debug.Log("Found " + duplicates.Length);
        foreach(var d in duplicates)
        {
            if (d.gameObject != collision.gameObject && d.GetComponent<Duplicate>() != null)
            {
                d.GetComponent<Duplicate>().ApplyForce(point, ExplosionForce, HitRadius);// ExplosionForce * Vector3.Distance(d.transform.position, point) / HitRadius);

                //d.GetComponent<Rigidbody>().isKinematic = false;
//d.GetComponent<Rigidbody>().AddExplosionForce(ExplosionForce, point, HitRadius);
                //d.GetComponent<Duplicate>().ApplyForce(collision.contacts[0].point, MoveSpeed - Vector3.Distance(point, d.transform.position));
            }
        }

        if (HitEffect != null)
        {
            Destroy(Instantiate(HitEffect, collision.contacts[0].point, HitEffect.transform.rotation), 10f);
        }

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        Destroy(gameObject,2f);
        this.enabled = false;
    }    
}
