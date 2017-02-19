using UnityEngine;
using System.Collections;

public class Duplicate : BaseAddForceObject
{
    //public float minForce;
    public float Health = 150;

    //bool hit = false;
    float timer = 0f;
    Material mat;
    public bool CanSplit = true;

    // Use this for initialization
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody == null)
        {
            rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.isKinematic = true;
            rigidBody.mass = 5;
        }


    }
    
    public override void ApplyForce(Vector3 origin, float force, float radius)
    {
        base.ApplyForce(origin, force, radius);
        if(hit)
        {
            doCollision(origin, force);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!hit )// && collision.relativeVelocity.magnitude > minForce)
        {
            if(!doCollision(collision.contacts[0].point, collision.relativeVelocity.magnitude))
            {
                Health -= collision.relativeVelocity.magnitude;
                if(Health <= 0)
                {
                    hit = true;
                    rigidBody.isKinematic = false;
                    Destroy(gameObject, Random.Range( 10, 30));
                }
            }
        }
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
                        var t = Instantiate(transform, pos + scale.x * transform.right * i + scale.y * transform.up * j + scale.z * transform.forward * k, transform.rotation, transform.parent);
                        t.localScale = transform.localScale / 2.05f;
                        //t.GetComponent<Duplicate>().enabled = false;
                        t.GetComponent<Duplicate>().hit = true;
                        t.GetComponent<Duplicate>().CanSplit = transform.localScale.magnitude > .2f && Random.Range(0, 100) > 80;
                        t.GetComponent<Rigidbody>().isKinematic = false;
                        t.GetComponent<Rigidbody>().mass = rigidBody.mass/2;
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
