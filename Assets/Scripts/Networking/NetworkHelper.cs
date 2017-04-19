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
    public List<Mission> Missions = new List<Mission>();
    public List<NpcBase> Enemies = new List<NpcBase>();

    void Awake ()
    {
        instance = this;
        
        MyLocations.Add(Resources.LoadAll<Location>("")[0]);
        Enemies = Resources.LoadAll<NpcBase>("Enemies").ToList();
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

    public void SpawnEnemy(NpcBase e, InstantiatedLocation Owner)
    {
        var inst = Instantiate(e, Owner.transform);
        inst.SpawnEnemy(Owner, Owner.FindOpenSpotInLocation());
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
        var location = c.SpawnLocation(go.transform, seed);
        var pos = c.Position;
        pos.y = 0;
        go.transform.position = pos;
        
        if (c.Standing == LocationStandings.Hostile)
        {
            // spawn enemies here?
            // or instantiatedlocation or mission
        }
    }

    public void CreateMission(string name, int seed, int locType, int level)
    {
        if (isServer)
            RpcCreateMission(name, seed, locType, level);
        else
            CmdCreateMission(name, seed, locType, level);
    }

    [ClientRpc]
    void RpcCreateMission(string name, int seed, int locType, int level)
    {
        UnityEngine.Random.InitState(seed);

        Mission m = new GameObject(name).AddComponent<Mission>();
        m.Name = name;
        m.Seed = seed;
        m.LocationType = locType;
        m.Level = level;
        MyLocations.Add(m.InstantiateMission());

        Missions.Add(m);
    }

    [Command]
    void CmdCreateMission(string name, int seed, int locType, int level)
    {
        RpcCreateMission(name, seed, locType, level);
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