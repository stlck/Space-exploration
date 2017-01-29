using UnityEngine;
using System.Collections;

public class Duplicate : MonoBehaviour
{
    public float minForce;
    public float Health = 300;

    bool hit = false;
    float timer = 0f;
    Material mat;
    Rigidbody rigidBody;

    // Use this for initialization
    void Start()
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
        if (!hit && collision.relativeVelocity.magnitude > minForce)
        {
            Health -= collision.relativeVelocity.magnitude;
            if (rigidBody.isKinematic)
                rigidBody.isKinematic = false;
        }
    }

    void Update()
    {
        if(hit)
        {
            timer += Time.deltaTime;
            if(timer >= 3f)
            {
                //mat.SetFloat("_SliceAmount", timer - 3f);
                if (timer >= 4f)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if(Health<= 0)
        {
            timer = Random.Range(-2f, 2f);
            hit = true;
        }
    }
}
