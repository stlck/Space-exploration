using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponMount : MovementBase, CmdObj, IShipSpawnObject
{

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

    [SyncVar]
    public int TilePositionX;
    [SyncVar]
    public int TilePositionY;

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
        Mounted.Invoke(false);
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
        LocalYMin = -60;
        LocalYMax = 60;
        MouseLookAt = MountTarget.forward;
        Mounted.Invoke(false);
        setInitialRotation();
    }
    
    void setInitialRotation()
    {
        // find out where the is solid behind gun. point the transform other way, limit to -60 -> 60
        
        // go clockwise until last 1, that is limit y min
        var owner = GetComponentInParent<Ship>();
        var center = Vector3.right * owner.Sizex / 2 + Vector3.forward * owner.Sizey / 2;
        //var localx = (int)transform.localPosition.x + (int)center.x;
        //var localz = (int)transform.localPosition.z + (int)center.z;
        var top = true;
        var bottom = true;
        var left = true;
        var right = true;
        

        for (int i = -2; i <= 2; i++)
        {
            if (owner.tiles[TilePositionX + i, TilePositionY - 2] == 1)
                bottom = false;
            if (owner.tiles[TilePositionX + i, TilePositionY + 2] == 1)
                top = false;
        }
        for (int j = -2; j <= 2; j++)
        {
            if (owner.tiles[TilePositionX + 2, TilePositionY + j] == 1)
                left = false;
            if (owner.tiles[TilePositionX - 2, TilePositionY + j] == 1)
                right = false;
        }

        if(top)
        {
            //transform.LookAt(transform.TransformPoint(Vector3.right + Vector3.forward));
        }
        else if(right)
        {
            transform.eulerAngles = Vector3.up * -90;
            //transform.LookAt(transform.TransformPoint(Vector3.right));
        }
        else if(left)
        {
            transform.eulerAngles = Vector3.up * 90;
            //transform.LookAt(transform.TransformPoint(Vector3.left));

        }
        else if(bottom)
        {
            //transform.LookAt(transform.TransformPoint(Vector3.back));
            transform.eulerAngles = Vector3.up * 180;
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

    public List<Vector2Int> TileConfig()
    {
        var mustHaveAZero = new List<Vector2Int>();
        mustHaveAZero.Add(new Vector2Int(-1, 0));
        mustHaveAZero.Add(new Vector2Int(1, 0));
        mustHaveAZero.Add(new Vector2Int(0, -1));
        mustHaveAZero.Add(new Vector2Int(0, 1));

        return mustHaveAZero;
    }

    public void SetTilePosition(Vector2Int position)
    {
        TilePositionX = position.x;
        TilePositionY = position.y;
    }
}
