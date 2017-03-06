using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionMessage : MonoBehaviour
{
    public delegate void CollisionEnter(Collision c);
    public event CollisionEnter CollisionEntered;
        
    public void OnCollisionEnter(Collision collision)
    {
        if(CollisionEntered != null)
            CollisionEntered(collision);
    }
}
