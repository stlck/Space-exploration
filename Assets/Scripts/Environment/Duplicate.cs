using UnityEngine;
using System.Collections;

public class Duplicate : MonoBehaviour
{
    public float minForce;
    public float Health = 150;

    bool hit = false;
    float timer = 0f;
    Material mat;
    Rigidbody rigidBody;

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
        //mat = GetComponent<MeshRenderer>().material;
        //mat.SetTextureOffset("_MainTex", new Vector2(((float)Random.Range(1,5))/4f, ((float)Random.Range(1, 5)) / 4f));

    }
    
    public void OnCollisionEnter(Collision collision)
    {
        if (enabled && !hit && collision.relativeVelocity.magnitude > minForce)
        {
            /* transform.localScale = transform.localScale * .95f;
             Health -= collision.relativeVelocity.magnitude;
             if (rigidBody.isKinematic)
                 rigidBody.isKinematic = false;*/
            var pos = transform.position;
            var scale = transform.localScale / 4f;
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    for (int k = 0; k < 2; k++)
                    {
                        var t = Instantiate(transform, pos + scale.x * transform.right * i + scale.y * transform.up * j + scale.z * transform.forward * k, transform.rotation, transform.parent);
                        t.localScale = transform.localScale / 2.05f;
                        t.GetComponent<Duplicate>().enabled = false;
                        t.GetComponent<Rigidbody>().isKinematic = false;
                        t.GetComponent<Rigidbody>().mass = 2;
                        Destroy(t.gameObject, Random.Range(4, 16));
                    }

            Destroy(gameObject);
        }
    }

   /* void Update()
    {
        if(hit)
        {
            timer += Time.deltaTime;
            if(timer >= 3f)
            {
                //mat.SetFloat("_SliceAmount", timer - 3f);
                if (timer >= 4f)
                {
                    var pos = transform.position;
                    var scale = transform.localScale/4f;
                    for(int i = 0; i < 2; i++)
                        for(int j = 0; j < 2; j++)
                            for(int k = 0; k < 2; k++)
                        {
                                var t = Instantiate(transform, pos + scale.x * transform.right * i + scale.y * transform.up * j + scale.z * transform.forward * k, transform.rotation);
                                t.localScale = transform.localScale / 2;
                                t.GetComponent<Duplicate>().enabled = false;
                                Destroy(t.gameObject, Random.Range(4, 8));
                        }
                    Destroy(gameObject);
                }
            }
        }
        else if(Health<= 0)
        {
            timer = Random.Range(-2f, 2f);
            hit = true;
        }
    }*/
}
