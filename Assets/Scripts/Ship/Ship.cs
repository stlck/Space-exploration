using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class Ship : NetworkBehaviour {

    public BoolUpdate ActiveStatus;

    public bool Warping = false;
    public Vector3 WarpPosition;
    public float WarpSpeed = 0;

    public bool Docking = false;
    public Transform AlignToTarget;

    public float CurrentSpeed = 0;
    public MoveShip ShipMovement;

    public int[,] tiles;
    public int[,] controls;
    public int Sizex;
    public int Sizey;

    public Transform WarpEffect;
    public float WarpTime = 2f;
    float warpTimer = 0f;
    public Material ShipMaterial;

    public List<NetworkSpawnObject> NetworkSpawnObjects;

    void Awake()
    {
        transform.localScale = Vector3.one * 2;

    }

	// Use this for initialization
	void Start ()
    {
        if (isServer)
        {
            if (tiles.Length > 0)
            {
                var t = new int[Sizex * Sizey];
                int counter = 0;
                var output = "";
                for (int i = 0; i < Sizex; i++)
                    for (int j = 0; j < Sizey; j++)
                    {
                        output += tiles[i, j];

                        t[counter] = tiles[i, j];
                        counter++;
                    }
                //Debug.Log(output);
                RpcBuildShip(output, Sizex, Sizey);
            }
        }

    }

    [ClientRpc]
    void RpcParentToTransform(GameObject parent, GameObject child)
    {
        child.transform.SetParent(parent.transform, true);
    }
    
    [ClientRpc]
    public void RpcBuildShip(string shipString, int sizex, int sizey)
    {
        var counter = 0;
        var output = "";
        Sizex = sizex;
        Sizey = sizey;

        int[] t = new int [ shipString.Length];
        tiles = new int[sizex, sizey];
        for (int i = 0; i < shipString.Length; i++)
            t[i] = (int)char.GetNumericValue(shipString[i]);

        for (int i = 0; i < Sizex; i++)
            for (int j = 0; j < Sizey; j++)
            {
                tiles[i, j] = t[counter];
                output += tiles[i, j];
                counter++;
            }
        Debug.Log(shipString);
        Debug.Log(output);

        var mTarget = transform;// GetComponentInChildren<MeshFilter>();
        ShipSpawner.ShipToMesh(mTarget, sizex, sizey, tiles,ShipMaterial);
        mTarget.gameObject.layer = LayerMask.NameToLayer("Ship");
    }

    public void WarpTo(Vector3 Position)
    {
        if (isServer)
        {
            ServerWarp(Position);
        }
        else
            MyAvatar.Instance.CmdWarpShip(netId, Position);
    }

    [Server]
    public void ServerWarp(Vector3 Position)
    {
        Warping = true;
        WarpPosition = Position;
        warpTimer = 0f;

        RpcDoWarpEffect();
    }

    [ClientRpc]
    public void RpcDoWarpEffect()
    {
        if(WarpEffect != null)
        { 
            var e = Instantiate(WarpEffect, transform.position, WarpEffect.rotation);
            Destroy(e.gameObject, WarpTime + 1);
        }
    }

    // Update is called once per frame
    [Server]
	void Update () {

        if (Warping)
        {
            warpTimer += Time.deltaTime;
            if(warpTimer >= WarpTime)
            {
                Warping = false;
                transform.position = WarpPosition;
            }
           /* var sign = Vector3.Cross(transform.forward, WarpPosition - transform.position).z < 0 ? -1: 1;
            transform.Rotate(Vector3.up, Mathf.Clamp(sign * Vector3.Angle(transform.forward, WarpPosition - transform.position),-ShipMovement.RotateSpeed,ShipMovement.RotateSpeed) * Time.deltaTime);
            //transform.forward = Vector3.RotateTowards(transform.forward, transform.position - WarpPosition, 0, );
            if(Vector3.Angle(transform.forward, WarpPosition - transform.position) < 10)
            {
                WarpSpeed = Mathf.SmoothStep(CurrentSpeed, WarpSpeed, 3);
                var t= Vector3.MoveTowards(transform.position, WarpPosition, WarpSpeed * Time.deltaTime);
                WarpPosition.y = 30;
                t.y = 30;
                transform.position = t;

                if (Vector3.Distance(transform.position, WarpPosition) < 50)
                {
                    t.y = 0;
                    transform.position = t;
                    Warping = false;
                    CurrentSpeed = 0;
                }
            }*/
        }

        else if(Docking)
        {
            transform.position = Vector3.MoveTowards(transform.position, AlignToTarget.position, 5 * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, AlignToTarget.rotation, 10 * Time.deltaTime);
        }

        //if (isServer)
        RpcUpdatePosition(transform.position, transform.eulerAngles);
	}

    [ClientRpc]
    public void RpcUpdatePosition(Vector3 pos, Vector3 rot)
    {
        transform.position = pos;
        transform.eulerAngles = rot;
    }

    public void ActivateShip()
    {
        ActiveStatus.Invoke(true);
    }

    public void DeActivateShip()
    {
        ActiveStatus.Invoke(false);
    }
}

interface IShipSpawnObject
{
    List<Vector2Int> TileConfig();
    void SetTilePosition(Vector2Int position);
}