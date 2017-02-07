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
        if (ShipWeapon == null)
            return;
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
        if(ShipWeapon == null)
        {
            //show weapon list
            // send equip command to server
        }
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
        LocalYMin = LocalYMax = 0;
        MouseLookAt = MountTarget.forward;
        Mounted.Invoke(false);
        calculateLimits();
    }
    float calcAngle(int x, int y)
    {
        var norm = Vector3.right * (x) + Vector3.forward * (y);
        norm.Normalize();
        var angle = Vector3.Angle(Vector3.forward, norm);
        var cross = Vector3.Cross(Vector3.forward, norm);
        if (cross.y < 0)
            angle = -angle;

        return angle;
    }
    void calculateLimits()
    {
        var owner = GetComponentInParent<Ship>();
        var center = Vector3.right * owner.Sizex / 2 + Vector3.forward * owner.Sizey / 2;
        var localx = (int)transform.localPosition.x;
        var localz = (int)transform.localPosition.z;
        for (int i = -2; i <= 2; i++)
        {
            if(owner.tiles[i + localx + (int)center.x, 2 + localz + (int)center.y] == 0)
            {
                var angle = calcAngle(i, 2);

                if (angle < LocalYMin)
                    LocalYMin = angle;
                if (angle > LocalYMax)
                    LocalYMax = angle;
            }
            else if (owner.tiles[i + localx + (int)center.x, -2 + localz + (int)center.y] == 0)
            {
                var angle = calcAngle(i, -2);

                if (angle < LocalYMin)
                    LocalYMin = angle;
                if (angle > LocalYMax)
                    LocalYMax = angle;
            }
        }
        for (int j = -2; j <= 2; j++)
        {
            if(owner.tiles[2 + localx + (int)center.x, j + localz + (int)center.y] == 0)
            {
                var angle = calcAngle(2,j);

                if (angle < LocalYMin)
                    LocalYMin = angle;
                if (angle > LocalYMax)
                    LocalYMax = angle;
            }
            if (owner.tiles[-2 + localx + (int)center.x, j + localz + (int)center.y] == 0)
            {
                var angle = calcAngle(-2,j);

                if (angle < LocalYMin)
                    LocalYMin = angle;
                if (angle > LocalYMax)
                    LocalYMax = angle;
            }
            //var ang = Vector3.Angle(transform.forward, Vector3.right * i + Vector3.forward * j - Vector3.right * localx + Vector3.forward * localz);
            //if (i != localx && j != localz)
            //{
            //    if (ang < LocalYMin)
            //        LocalYMin = ang;
            //    else if (ang > LocalYMax)
            //        LocalYMax = ang;
            //}
        }
    }

    void calcAngleLimit2()
    {
        // find out where the is solid behind gun. point the transform other way, limit to -60 -> 60



        // go clockwise until last 1, that is limit y min
        //var owner = GetComponentInParent<Ship>();
        //var center = Vector3.right * owner.Sizex / 2 + Vector3.forward * owner.Sizey / 2;
        //var localx = (int)transform.localPosition.x + (int)center.x;
        //var localz = (int)transform.localPosition.z + (int)center.y;
        //var top = false;
        //var bottom = false;
        //var left = false;
        //var right = false;
        

        //for (int i = -2; i <= 2; i++)
        //{
        //    if (owner.tiles[localx + i, localz - 2] == 1)
        //        top = false;
        //    if (owner.tiles[localx + i, localz + 2] == 1)
        //        bottom = false;
        //}
        //for (int j = -2; j <= 2; j++)
        //{
        //    if (owner.tiles[localx + 2, localz + j] == 1)
        //        left = false;
        //    if (owner.tiles[localx - 2, localz + j] == 1)
        //        right = false;
        //}

        //if(top)
        //{
        //    LocalYMin = -45;
        //    LocalYMax = 45;
        //    if(left)
        //    {
        //        LocalYMin -= 90;
        //    }
        //    if (right)
        //    {
        //        LocalYMax += 90;
        //    }
        //}
        //else if(right)
        //{
        //    LocalYMin = 45;
        //    LocalYMax = 135; // or -120?
        //    if (bottom)
        //        LocalYMax += 90;
        //    if (bottom && left)
        //        LocalYMax += 90;
        //}
        //else if(left)
        //{
        //    LocalYMin = -45;
        //    LocalYMax = -135;
        //    if (bottom)
        //        LocalYMin -= 90;
        //}
        //else if(bottom)
        //{
        //    LocalYMin = 180 - 45;
        //    LocalYMax = 180 + 45;
        //}
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
