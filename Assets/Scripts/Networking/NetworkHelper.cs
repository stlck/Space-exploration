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
    public List<Location> SpawnedLocations = new List<Location>();
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
        var go = Instantiate(so.SpawnTarget);
        go.transform.SetParent(so.Parent.transform);
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

        Testing.AddDebug("Spawned NetworkObject: " + go.name);

        return go;
    }

    public void SpawnEnemy(NpcBase e, InstantiatedLocation Owner)
    {
        var inst = Instantiate(e, Owner.transform);
        var pos = Owner.FindOpenSpotInLocation();
        inst.SpawnEnemy(Owner, pos);

        Testing.AddDebug("Spawned Enemy: " + e.name + " at " + pos);
    }

    [ClientRpc]
    void RpcParentSpawnObject(GameObject go, GameObject parent)
    {
        go.transform.SetParent(parent.transform, true);
    }
    
    [ClientRpc]
    public void RpcSpawnLocation(string locationName, int seed)
    {
        SpawnLocation(MyLocations.First(m => m.Name == locationName), seed);
    }

    public void SpawnLocation(string locName, int seed)
    {
        SpawnLocation(MyLocations.First(m => m.Name == locName), seed);
    }

    public InstantiatedLocation SpawnLocation(Location loc, int seed)
    {
        //var c = MyLocations.First(m => m.Name == locationName);
        var go = new GameObject(loc.name);
        var location = loc.SpawnLocation(go.transform, seed);
        location.name = loc.name;
        var pos = loc.Position;
        //pos.y = -.5f;
        go.transform.position = pos;

        Testing.AddDebug("Spawned location: " + loc.name + " at " + pos);
        SpawnedLocations.Add(loc);
        return location;
    }

    public void SpawnMission(string Name)
    {
        var mission = Missions.First(m => m.Name == Name);
        var location = SpawnLocation(mission.Location, mission.Seed);

        if (isServer && mission.Location.Standing == LocationStandings.Hostile)
        {
            SpawnEnemies(mission, location);
            // spawn enemies here?
            // or instantiatedlocation or mission
        }
        Testing.AddDebug("Mission spawned: " + Name);
    }

    public void SpawnEnemies(Mission target, InstantiatedLocation owner)
    {
        var amountToSpawn = 10 + (target.Level + 1) * UnityEngine.Random.Range(8,16);
        var spawned = 0;
        while(spawned <= amountToSpawn)
        {
            var e = Enemies.GetRandom();
            SpawnEnemy(e, owner);
            spawned += e.GetComponent<StatBase>().CreditsOnKill;
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
    void RpcCreateMission(string Name, int seed, int locType, int level)
    {
        UnityEngine.Random.InitState(seed);

        Mission m = new GameObject(Name).AddComponent<Mission>();
        m.Name = Name;
        m.Seed = seed;
        m.LocationType = locType;
        m.Level = level;
        var loc = m.InstantiateMission();
        MyLocations.Add(loc);

        Missions.Add(m);
        Testing.AddDebug("Added Mission " + Name);
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

public static class StaticExtentions
{
    public static T GetRandom<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}