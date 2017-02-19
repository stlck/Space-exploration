﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAddForceObject : MonoBehaviour {

    public float minForce;

    public Rigidbody rigidBody;
    public bool hit = false;

    // Use this for initialization
    void Start () {
		if(rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
            if(rigidBody == null)
            {
                rigidBody = gameObject.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
                rigidBody.mass = 5;
            }
        }
	}

    public virtual void ApplyForce(Vector3 origin, float force, float radius)
    {
        if (!hit)
        {
            var dist = Vector3.Distance(origin, transform.position);
            dist /= radius;
            dist = Mathf.Clamp(dist, 0, 1);
            dist = 1 - dist;
            var tForce = force * dist;
            if (tForce >= minForce)
            {
                hit = true;
            }
        }

        if (hit)
        {
            rigidBody.AddExplosionForce(force, origin, radius);
        }
    }
}
