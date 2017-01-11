using System;
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

}
