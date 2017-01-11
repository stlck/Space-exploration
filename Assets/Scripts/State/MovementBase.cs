using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementBase : NetworkBehaviour {

    public float vert;
    public float hor;

    public Vector3 MouseLookAt;

    public virtual void SetInput(float inVert, float inHor)
    {
        vert = inVert;
        hor = inHor;
    }

    public virtual void SetLookingAt(Vector3 m)
    {
        MouseLookAt = m;
    }
}
