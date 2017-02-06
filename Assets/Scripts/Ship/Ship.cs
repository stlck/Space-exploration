using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

    public List<NetworkSpawnObject> NetworkSpawnObjects;

	// Use this for initialization
	void Start () {
		if(isServer)
        {
            if(tiles.Length > 0)
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
                RpcBuildShip(t, Sizex, Sizey);
            }
            NetworkSpawnObjects.ForEach(m => NetworkHelper.Instance.NetworkSpawnObject(m));
        }
	}

    [ClientRpc]
    void RpcParentToTransform(GameObject parent, GameObject child)
    {
        child.transform.SetParent(parent.transform, true);
    }
    
    [ClientRpc]
    public void RpcBuildShip(int[] t, int sizex, int sizey)
    {
        var counter = 0;
        var output = "";
        for (int i = 0; i < Sizex; i++)
            for (int j = 0; j < Sizey; j++)
            {
                tiles[i, j] = t[counter];
                        output += tiles[i, j];
                counter++;
            }
                //Debug.Log(output);

        var mTarget = GetComponentInChildren<MeshFilter>();
        mTarget.gameObject.AddComponent<MeshCollider>().sharedMesh = mTarget.GetComponent<MeshFilter>().mesh;
        mTarget.gameObject.layer = LayerMask.NameToLayer("Ship");
        ShipSpawner.ShipToMesh(mTarget, sizex, sizey, tiles);
    }

    // Update is called once per frame
    [Server]
	void Update () {

        if (Warping)
        {
            var sign = Vector3.Cross(transform.forward, WarpPosition - transform.position).z < 0 ? -1: 1;
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
            }
        }

        if(Docking)
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
