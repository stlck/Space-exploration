using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    public BaseWeapon Owner;
    public GameObject HitEffect;
    public float MoveSpeed = 10;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("bullet at " + transform.position);

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
        if ( MyAvatar.Instance.GetComponent<Collider>() == collision.collider)
        {
            Physics.IgnoreCollision(collision.collider, this.GetComponent<Collider>(), true);
            return;
        }

        Debug.Log("Collision " + collision.gameObject.name, collision.gameObject);
        if(MyAvatar.Instance.isServer)
        { 
            Debug.Log("Collision on server");
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
