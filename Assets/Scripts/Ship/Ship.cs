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

    public List<GameObject> SpawnObjects;
    public List<NetworkSpawnObject> NetworkSpawnObjects;

	// Use this for initialization
	void Start () {
		if(isServer)
        {
            SpawnObjects.ForEach(m =>
            {
                //GameObject i = Instantiate(m, transform);
                //i.transform.localPosition = m.transform.position;
                //NetworkServer.Spawn(i);
                //RpcParentToTransform(gameObject, i);
            });

            NetworkSpawnObjects.ForEach(m => NetworkHelper.Instance.NetworkSpawnObject(m));
        }

	}

    [ClientRpc]
    void RpcParentToTransform(GameObject parent, GameObject child)
    {
        child.transform.SetParent(parent.transform, true);
    }
	
	// Update is called once per frame
    [Server]
	void Update () {

        if (Warping)
        {
            transform.Rotate(Vector3.up, Mathf.Clamp(Vector3.Angle(transform.forward, WarpPosition - transform.position),-ShipMovement.RotateSpeed,ShipMovement.RotateSpeed) * Time.deltaTime);
            //transform.forward = Vector3.RotateTowards(transform.forward, transform.position - WarpPosition, 0, );
            if(Vector3.Angle(transform.forward, WarpPosition - transform.position) < 10)
            {
                WarpSpeed = Mathf.SmoothStep(CurrentSpeed, WarpSpeed, 3);

                transform.position = Vector3.MoveTowards(transform.position, WarpPosition, WarpSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, WarpPosition) < 50)
                {
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
