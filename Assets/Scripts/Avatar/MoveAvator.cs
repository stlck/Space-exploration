﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MoveAvator : MovementBase
{
    public float ForwardSpeed = 5;
    public float StrafeSpeed = 2;
    public LayerMask walkable;
    public NetworkTransform nTransform;
    public Transform XAxisRotationTransform;
    CharacterController charCtrl;

    // Use this for initialization
    void Start () {
        charCtrl = GetComponent<CharacterController>();
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
        if(charCtrl != null)
        {
            if(isServer || isLocalPlayer && nTransform.lastSyncTime > Time.deltaTime)
            {
                var forward = Vector3.forward * vert * ForwardSpeed;
                var right = Vector3.right * hor * StrafeSpeed;
                var grav = Vector3.zero;

                if(Physics.Raycast(transform.position, Vector3.down, 2, walkable))
                {
                    grav += Physics.gravity;
                }
                else
                {
                    grav = Vector3.down * (transform.position.y - .51f) * Time.deltaTime;
                    forward /= 3;
                    right /= 3;
                }
                
                charCtrl.Move((forward + right + grav) * Time.deltaTime);
                rotateTransform();
            }
        }
        else if(isServer || isLocalPlayer && nTransform.lastSyncTime > Time.deltaTime)
        {
            var forward = Vector3.forward * vert * ForwardSpeed;
            var right = Vector3.right * hor * StrafeSpeed;

            RaycastHit hit;

            if (Physics.CheckBox(transform.position + forward.normalized + Vector3.down / 2, Vector3.forward + Vector3.up, Quaternion.identity, walkable.value) && 
                !Physics.Raycast( new Ray(transform.position, forward), .5f, walkable))
                transform.position = transform.position + (forward * Time.deltaTime);

            if (Physics.CheckBox(transform.position + right.normalized + Vector3.down / 2, Vector3.forward + Vector3.up, transform.rotation, walkable.value) && 
                !Physics.Raycast(new Ray(transform.position, right), .5f, walkable))
                transform.position = transform.position + (right * Time.deltaTime);

                rotateTransform();
        }
    }

    void rotateTransform()
    {
        var lookAt = MouseLookAt;
        lookAt.y = transform.position.y;
        transform.LookAt(lookAt);
        XAxisRotationTransform.LookAt(MouseLookAt);
        //XAxisRotationTransform
    }
}
