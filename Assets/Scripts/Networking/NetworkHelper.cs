using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public List<Location> MyLocations = new List<Location>();

    void Awake()
    {
        instance = this;
        MyLocations = Resources.LoadAll<Location>("").ToList();
    }

    // Use this for initialization
    void Start()
    {
        NetworkManager.singleton.runInBackground = true;
    }

    public void SpawnSpaceEncounter(Location t)
    {
        CmdSpaceEncounter(t);
    }

    [Command]
    public void CmdSpaceEncounter(Location t)
    {

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

    [ClientRpc]
    public void RpcSpawnLocation(string locationName, int seed)
    {
        var c = MyLocations.First(m => m.Name == locationName);
        var go = new GameObject(c.name);
        c.SpawnLocation(go.transform, seed);
        go.transform.position = c.Position;
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