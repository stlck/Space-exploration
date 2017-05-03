using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShipCollisions : MonoBehaviour {

    public Ship Owner;
    public List<Vector3> CurrentCollisionNormal;
    public List<Vector3> TriggerBoundsToched;

    void OnCollisionStay (Collision collisionInfo)
    {
        CurrentCollisionNormal = collisionInfo.contacts.Where(m => m.otherCollider.transform.root != Owner.transform).Select(m => m.normal * -1).ToList();
    }

    void OnTriggerStay (Collider other)
    {
        
        //other.ClosestPointOnBounds
        //CurrentCollisionNormal = collisionInfo.contacts.Select(m => m.normal * -1).ToList();
    }

    void OnCollisionEnter (Collision collision)
    {
        
    }

    void OnCollisionExit (Collision collisionInfo)
    {

    }

    void OnDrawGizmos()
    {
        foreach(var c in CurrentCollisionNormal)
        {
            Gizmos.DrawLine(transform.position + Vector3.up * 5, transform.position + Vector3.up * 5 + c * 5);
        }
    }
}
