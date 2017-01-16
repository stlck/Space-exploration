using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementBase : NetworkBehaviour {

    public float vert;
    public float hor;

    public Vector3 MouseLookAt;
    public bool IsMouseDown = false;

    public virtual void SetInput(float inVert, float inHor)
    {
        vert = inVert;
        hor = inHor;
    }

    public virtual void SetLookingAt(Vector3 m)
    {
        MouseLookAt = m;
    }

    public virtual void SetMouseDown(bool val)
    {
        IsMouseDown = val;
    }

    public virtual void ReleaseControl()
    {
        vert = 0;
        hor = 0;
    }

    public virtual void TakeControl()
    {

    }
}
