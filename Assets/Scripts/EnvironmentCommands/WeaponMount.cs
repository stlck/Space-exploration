using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponMount : MovementBase, CmdObj {

    public float RotateSpeed = 3;
    [SyncVar]
    public int InControl = -1;
    public BoolUpdate Mounted;

    public Transform MountTarget;

    public float LocalYMin = -60;
    public float LocalYMax = 60;
    public Vector3 LookTarget;

    public ShipWeapon ShipWeapon;
    MyAvatar currentOperator;

    public bool canExecuteCommand()
    {
        return true;
    }

    public void doCommand(int senderId)
    {
        if(InControl == senderId)
        {
            currentOperator = null;
        }
        else if(InControl == -1)
        {
            InControl = senderId;

            currentOperator = NetworkHelper.Instance.AllPlayers.Find(m => m.PlayerId == senderId);
            currentOperator.SetMovementInput(this);

            //currentOperator.MyInput.EventMouseDown += MyInput_EventMouseDown;
        }
    }

    public void localCommand()
    {
        if (InControl == -1)
            Mounted.Invoke(true);
        else
            Mounted.Invoke(false);
    }

    public override void TakeControl()
    {
        base.TakeControl();
    }

    public override void ReleaseControl()
    {
        base.ReleaseControl();
        
        InControl = -1;
        localCommand();
        currentOperator = null;
    }

    [ClientRpc]
    void RpcClientFire()
    {
        //Debug.Log("ClientFIre");
        ShipWeapon.FireWeapon();
    }

    // Use this for initialization
    void Start () {
        MouseLookAt = MountTarget.forward;
        Mounted.Invoke(false);
        calculateLimits();
    }

    void calculateLimits()
    {
        var owner = GetComponentInParent<Ship>();
        var localx = (int)transform.localPosition.x;
        var localz = (int)transform.localPosition.z;
        for(int i = localx -1; i <= localx +1; i++)
            for(int j = localz-1; j <= localz + 1; j++)
                {
                    
                    var ang = Vector3.Angle(transform.forward, Vector3.right * i + Vector3.forward * j - Vector3.right * localx + Vector3.forward * localz);
                    if (i != localx && j != localz)
                    {
                        if (ang < LocalYMin)
                            LocalYMin = ang;
                        else if (ang > LocalYMax)
                            LocalYMax = ang;
                    }
                }
    }

    // Update is called once per frame
    void Update () {

	}
    float ClampAngle(float angle, float from, float to)
    {
        if (angle > 180) angle = 360 - angle;
        angle = Mathf.Clamp(angle, from, to);
        if (angle < 0) angle = 360 + angle;

        return angle;
    }

    public override void SetMouseDown(bool val)
    {
        base.SetMouseDown(val);

        if(val && ShipWeapon.CanFire())
        {
            RpcClientFire();
            //ShipWeapon.FireWeapon();
        }
    }

    public override void SetLookingAt(Vector3 m)
    {
        base.SetLookingAt(m);

        LookTarget = MouseLookAt - MountTarget.position;
        MountTarget.forward = Vector3.RotateTowards(MountTarget.forward, LookTarget.normalized, RotateSpeed * Time.deltaTime, RotateSpeed * Time.deltaTime);

        var e = MountTarget.localEulerAngles;
        e.x = 0;
        e.z = 0;
        e.y = Mathf.Clamp(Mathf.DeltaAngle(-e.y, 0), LocalYMin, LocalYMax);
        MountTarget.localEulerAngles = e;
    }
}
