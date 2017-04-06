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
        if (!NewCollision)
        { 
            rigidBody = GetComponent<Rigidbody>();
            if (rigidBody == null)
            {
                rigidBody = gameObject.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
                rigidBody.mass = 5;
            }
        }
    }

    public override void Start ()
    {
        
    }

    public override void ApplyForce(Vector3 origin, float force, float radius)
    {
        base.ApplyForce(origin, force, radius);
        
        if(hit)
        {
            if (!NewCollision)
                doCollision(origin, force);
            else
                newCollisionTest(origin, force * Vector3.Distance(transform.position, origin) / radius);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!NewCollision)
        {
            if (!hit )// && collision.relativeVelocity.magnitude > minForce)
            {
                if (!doCollision(collision.contacts[0].point, collision.relativeVelocity.magnitude))
                {
                    Health -= collision.relativeVelocity.magnitude;
                    if (Health <= 0)
                    {
                        hit = true;
                        rigidBody.isKinematic = false;
                        Destroy(gameObject, Random.Range(10, 30));
                    }
                }
            }
        }
        else
        {
            newCollisionTest(collision.contacts[0].point, collision.relativeVelocity.magnitude);
        }
    }

    bool newCollisionTest(Vector3 originPoint, float damage)
    {
        var pos = transform.position;
        Health -= damage;

        if (Health <= 0)
        {
            //hit = true;
            //rigidBody.isKinematic = false;
            if (EffectOnDeath != null)
            {
                var e = Instantiate(EffectOnDeath, originPoint, EffectOnDeath.transform.rotation);
                e.SetMaterial(mat);
                //t = fragment.transform;
            }
            Destroy(gameObject);
        }
        else if(EffectOnHit != null)
        {
            var e = Instantiate(EffectOnHit, originPoint, EffectOnHit.transform.rotation);
            e.SetMaterial(mat);
        }

        return true;
    }

    bool doCollision(Vector3 originPoint, float force)
    {
        if (force < minForce)
        {
            return false;
        }
        
        hit = true;
        var pos = transform.position;
        var scale = transform.localScale / 4f;

        if(CanSplit)
        { 
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        if(Random.Range(0,100) > 40)
                        {
                            Transform t;
                            if (SpawnOnHit != null)
                            {
                                var fragment = Instantiate(SpawnOnHit, pos + scale.x * transform.right * i + scale.y * transform.up * j + scale.z * transform.forward * k, transform.rotation, transform.parent);
                                fragment.SetMaterial( GetComponent<InstanceMe>().Material);
                                t = fragment.transform;
                            }
                            else
                            {
                                t = Instantiate(transform, pos + scale.x * transform.right * i + scale.y * transform.up * j + scale.z * transform.forward * k, transform.rotation, transform.parent);
                                t.localScale = transform.localScale / 2.05f;

                                t.GetComponent<Duplicate>().hit = true;
                                t.GetComponent<Duplicate>().CanSplit = transform.localScale.magnitude > .2f && Random.Range(0, 100) > 80;
                                t.GetComponent<Rigidbody>().isKinematic = false;
                                t.GetComponent<Rigidbody>().mass = rigidBody.mass / 2;
                            }

                            if (t.gameObject.layer != LayerMask.NameToLayer("Ship"))
                                t.gameObject.layer = LayerMask.NameToLayer("Ship");
                            Destroy(t.gameObject, Random.Range(20, 50));
                        }
                    }

            Destroy(gameObject);
        }
        return true;
    }
    
}
