﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class NetworkHelper : NetworkBehaviour
{
    private static NetworkHelper instance;
    public static NetworkHelper Instance
    {
        get
        {
            return instance;
        }
    }

    public List<MyAvatar> AllPlayers = new List<MyAvatar>();

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start()
    {
        NetworkManager.singleton.runInBackground = true;
    }

    public void SpawnObject(GameObject go)
    {
        if (isServer)
            NetworkServer.Spawn(go);
    }

    [Server]
    public GameObject NetworkSpawnObject(NetworkSpawnObject so)
    {
        var go = Instantiate(so.SpawnTarget, so.Parent.transform);
        if (so.PositionIsLocal)
            go.transform.localPosition = so.Position;
        else
            go.transform.position = so.Position;

        if (so.EulersIsLocal)
            go.transform.localEulerAngles = so.Eulers;
        else
            go.transform.eulerAngles = so.Eulers;

        NetworkServer.Spawn(go);
        RpcParentSpawnObject(go, so.Parent);

        return go;
    }

    [ClientRpc]
    void RpcParentSpawnObject(GameObject go, GameObject parent)
    {
        go.transform.SetParent(parent.transform, true);
    }
}

[Serializable]
public class NetworkSpawnObject
{
    public GameObject SpawnTarget;
    public GameObject Parent;
    public Vector3 Position;
    public bool PositionIsLocal = true;
    public Vector3 Eulers;
    public bool EulersIsLocal = true;
}