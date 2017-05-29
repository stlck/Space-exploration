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
    public List<InstantiatedLocation> SpawnedInstantiatedLocations = new List<InstantiatedLocation>();
    public List<Mission> Missions = new List<Mission>();
    public List<NpcBase> Enemies = new List<NpcBase>();

    //[SyncVar]
    public SyncListInt WeaponSeeds = new SyncListInt();
    //public int[] WeaponSeeds ;

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

    void WeaponSeedsChanged (SyncList<int>.Operation op, int itemIndex)
    {
        Debug.Log("weaponSeeds updated");
    }

    public override void OnStartServer ()
    {
        base.OnStartServer();

        //WeaponSeeds = new SyncListInt();
        WeaponSeeds.Callback = WeaponSeedsChanged;
        //WeaponSeeds = new int[8];
        for (int i = 0; i < 8; i++)
            WeaponSeeds.Add(UnityEngine.Random.Range(0, 40000));
    }

    public override void OnStartClient ()
    {
        base.OnStartClient();

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
        var go = new GameObject(loc.Name);
        var location = loc.SpawnLocation(go.transform, seed);
        location.name = loc.Name;
        var pos = loc.Position;
        go.transform.position = pos;

        Testing.AddDebug("Spawned location: " + loc.Name + " at " + pos);
        //SpawnedLocations.Add(loc);
        SpawnedInstantiatedLocations.Add(location);
        return location;
    }

    public void SpawnMission(string Name)
    {
        Testing.AddDebug("Testing to spawn Mission: " + Name);
        if(!SpawnedInstantiatedLocations.Any(m => m.name == Name))
        {
            var mission = Missions.First(m => m.Name == Name);
            var location = SpawnLocation(mission.Location, mission.Seed);

            if (isServer && mission.Location.Standing == LocationStandings.Hostile)
            {
                if (mission.Location.Standing == LocationStandings.Hostile)
                    SpawnEnemies(mission, location);
                else if (mission.Location.Standing == LocationStandings.Friendly)
                    ;
                // spawn enemies here?
                // or instantiatedlocation 
                // or mission
            }
            Testing.AddDebug("Mission spawned: " + Name);
        }
    }

    public void SpawnEnemies(Mission target, InstantiatedLocation owner)
    {
        var amountToSpawn = 10 + (target.Level + 1) * UnityEngine.Random.Range(20,40);
        var spawned = 0;
        
        if(target.Location.Type == LocationTypes.SpaceStation || target.Location.Type == LocationTypes.Asteroid)
        {
            while(spawned <= amountToSpawn)
            {
                var e = Enemies.GetRandom();
                SpawnEnemy(e, owner);
                spawned += e.GetComponent<StatBase>().CreditsOnKill;
            }
        }
        else if(target.Location.Type == LocationTypes.SpaceEncounter)
        {
            // spawn ships
        }
    }

    public void RemoveInstantiatedLocation(string name)
    {
        RpcRemoveInstantiatedLocation(name);
    }

    [ClientRpc]
    void RpcRemoveInstantiatedLocation (string name)
    {
        if(SpawnedInstantiatedLocations.Any( m => m.name == name))
        {
            Destroy(SpawnedInstantiatedLocations.First(m => m.name == name));
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
    public static T GetRandom<T>(this IList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T GetRandom<T>(this IEnumerable<T> list)
    {
        return list.ElementAt(UnityEngine.Random.Range(0, list.Count()));
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}