using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShipCollisions : MonoBehaviour {

    public Ship Owner;
    public List<Vector3> CurrentCollisionNormal;

    void OnCollisionStay (Collision collisionInfo)
    {
        CurrentCollisionNormal = collisionInfo.contacts.Select(m => m.normal * -1).ToList();
    }

    void OnTriggerStay (Collider other)
    {
        //CurrentCollisionNormal = collisionInfo.contacts.Select(m => m.normal * -1).ToList();
    }

    void OnCollisionEnter (Collision collision)
    {
        
    }

    void OnCollisionExit (Collision collisionInfo)
    {

    }
}
