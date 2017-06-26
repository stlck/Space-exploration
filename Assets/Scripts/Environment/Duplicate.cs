using UnityEngine;
using System.Collections;

public class Duplicate : BaseAddForceObject
{
    float timer = 0f;
    Material mat;

    //public float minForce;
    public float Health = 150;
    public DuplicateFragment SpawnOnHit;

    //public DuplicateFragment EffectOnDeath;
    public DuplicateFragment EffectOnHit;

    public int X;
    public int Y;
    public int Z;

    public InstantiatedLocation Owner;

    // Use this for initialization
    void Awake()
    {
        tag = "Block";
        if (GetComponent<InstanceMe>() != null)
            mat = GetComponent<InstanceMe>().Material;
    }

    public override void Start ()
    {
        
    }

    void OnDeath(Vector3 point)
    {
        if(Owner != null)
        {
            Owner.BlockHit(X, Y, Z);
        }

        //if (EffectOnDeath != null)
        //{
        //    var e = Instantiate(EffectOnDeath, transform.position, EffectOnDeath.transform.rotation);
        //    if (mat != null)
        //        e.SetColor(mat.color);
        //        //e.SetMaterial(mat);
        //}

        Destroy(gameObject);
    }

    public override void ApplyForce(Vector3 origin, float force, float radius)
    {
        base.ApplyForce(origin, force, radius);
        
        if(hit)
        {
            newCollisionTest(origin, force * Mathf.Abs( 1f - Vector3.Distance(transform.position, origin) / radius));
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        //newCollisionTest(collision.contacts[0].point, collision.relativeVelocity.magnitude);
    }

    public void ApplyDamage(Vector3 originPoint, float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            OnDeath(originPoint);
            //Destroy(gameObject);
        }
    }

    bool newCollisionTest(Vector3 originPoint, float damage)
    {
        var pos = transform.position;
        //Debug.Log("Added force : " + damage);
        ApplyDamage(originPoint, damage);

        if (EffectOnHit != null)
        {
            var e = Instantiate(EffectOnHit, originPoint, EffectOnHit.transform.rotation);
            if(mat != null)
                e.SetColor(mat.color);
                //e.SetMaterial(mat);
        }

        return true;
    }
}
