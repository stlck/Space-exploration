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

    public bool canExecuteCommand()
    {
        return true;
    }

    public void doCommand(int senderId)
    {
        if(InControl == senderId)
        {
            InControl = -1;
            var t = NetworkHelper.Instance.AllPlayers.Find(m => m.PlayerId == senderId);
            t.ReleaseControl();
            //Mounted.Invoke(false);
        }
        else if(InControl == -1)
        {
            InControl = senderId;
            var t = NetworkHelper.Instance.AllPlayers.Find(m => m.PlayerId == senderId);
            t.SetMovementInput(this);
            //Mounted.Invoke(true);
        }
    }

    public void localCommand()
    {
        if(InControl == -1)
            Mounted.Invoke(true);
        else
            Mounted.Invoke(false);

    }

    // Use this for initialization
    void Start () {
        MouseLookAt = MountTarget.forward;
        Mounted.Invoke(false);
    }

    // Update is called once per frame
    void Update () {
		if(isServer && InControl >= 0)
        {
            LookTarget = MouseLookAt - MountTarget.position;
            MountTarget.forward = Vector3.RotateTowards(MountTarget.forward, LookTarget.normalized, RotateSpeed * Time.deltaTime, RotateSpeed * Time.deltaTime);

            var e = MountTarget.localEulerAngles;
            e.x = 0;
            e.z = 0;
            e.y = Mathf.Clamp(e.y, LocalYMin, LocalYMax);
            MountTarget.localEulerAngles = e;
        }
	}

}
