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
    public Vector3 DocingPosition;
    public Quaternion DocingRotation;

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
    public float ShipScale = 2;
    void Awake()
    {
        transform.localScale = Vector3.one * ShipScale;

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

            var dock = GetComponentInChildren<DockShip>();
            if (dock != null)
                dock.doCommand(0);
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
        }

        else if(Docking)
        {
            transform.position = Vector3.MoveTowards(transform.position, DocingPosition/*AlignToTarget.position*/, 5 * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, DocingRotation/* AlignToTarget.rotation*/, 10 * Time.deltaTime);
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