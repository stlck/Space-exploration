using UnityEngine;
using System.Collections;

public class Duplicate : BaseAddForceObject
{
    //public float minForce;
    public float Health = 150;
    public DuplicateFragment SpawnOnHit;
    
    float timer = 0f;
    Material mat;
    public bool CanSplit = true;

    public bool NewCollision = false;
    public DuplicateFragment EffectOnDeath;
    public DuplicateFragment EffectOnHit;

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
            //hit = true;
            //rigidBody.isKinematic = false;
            if (EffectOnDeath != null)
            {
                var e = Instantiate(EffectOnDeath, originPoint, EffectOnDeath.transform.rotation);
                if(mat != null)
                    e.SetMaterial(mat);
                //t = fragment.transform;
            }
            Destroy(gameObject);
        }
    }

    bool newCollisionTest(Vector3 originPoint, float damage)
    {
        var pos = transform.position;
        Debug.Log("Added force : " + damage);
        ApplyDamage(originPoint, damage);

        if (EffectOnHit != null)
        {
            var e = Instantiate(EffectOnHit, originPoint, EffectOnHit.transform.rotation);
            if(mat != null)
                e.SetMaterial(mat);
        }

        return true;
    }
}
