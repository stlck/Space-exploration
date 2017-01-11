using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MoveAvator : MovementBase
{
    public float ForwardSpeed = 5;
    public float StrafeSpeed = 2;
    public LayerMask walkable;
    public NetworkTransform nTransform;

    // Use this for initialization
    void Start () {

    }

    public void EnableMovement()
    {
      //  input.Horizontal += Input_Horizontal;
      //  input.Vertical += Input_Vertical;
    }

    public void DisableMovement()
    {
      //  input.Horizontal -= Input_Horizontal;
      //  input.Vertical -= Input_Vertical;
    }

    void OnDisable()
    {
        vert = hor = 0;
    }

	// Update is called once per frame
	void Update () {
        //if (PlayerState.Instance.CurrentState == States.Avatar)
        if(isServer || isLocalPlayer && nTransform.lastSyncTime > Time.deltaTime)
        {
            Debug.Log("not synced");

            var forward = transform.forward * vert * ForwardSpeed;
            var right = transform.right * hor * StrafeSpeed;

            RaycastHit hit;

            if (Physics.CheckBox(transform.position + forward.normalized + Vector3.down / 2, Vector3.forward + Vector3.up, transform.rotation, walkable.value) && 
                !Physics.Raycast( new Ray(transform.position, forward), .5f, walkable))
                transform.position = transform.position + (forward * Time.deltaTime);

            if (Physics.CheckBox(transform.position + right.normalized + Vector3.down / 2, Vector3.forward + Vector3.up, transform.rotation, walkable.value) && 
                !Physics.Raycast(new Ray(transform.position, right), .5f, walkable))
                transform.position = transform.position + (right * Time.deltaTime);

            transform.LookAt(MouseLookAt);
        }
    }
}
