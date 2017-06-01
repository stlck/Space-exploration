using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CreateEnvironment : MonoBehaviour {

    public ProceduralBlocks BlockGenerator;
    public List<Transform> GroundTiles;

    //public List<Location> MyLocations = new List<Location>();
    public List<Location> SpawnedLocations = new List<Location>();

    // Use this for initialization
    void Start () {
        //MyLocations = Resources.LoadAll<Location>("").ToList();
        //if (MyAvatar.Instance.isServer)
            StartCoroutine(spawnNearbyLocations());
    }

    IEnumerator spawnNearbyLocations()
    {
        while(true)
        {
            yield return new WaitForSeconds(.2f);
            checkAndAddEnvironment();
        }
    }
    
    public void ForceUpdateEnvironment()
    {
        checkAndAddEnvironment();
    }

    void checkAndAddEnvironment()
    {
        var nearbyMissions = NetworkHelper.Instance.Missions.Where
            (m => Vector3.Distance(transform.position, m.Location.Position) < 200 && !NetworkHelper.Instance.SpawnedInstantiatedLocations.Any(loc => loc.name == m.Location.Name));
        if(nearbyMissions.Any())
        {
            var mission = nearbyMissions.First();
            NetworkHelper.Instance.SpawnMission(mission.Name);
            //SpawnedLocations.Add(mission.Location);
        }

        // spawn terrain in vicinity (local only)
        //var close = NetworkHelper.Instance.MyLocations.Where
        //    (m => Vector3.Distance(transform.position, m.Position) < 200 && !NetworkHelper.Instance.SpawnedInstantiatedLocations.Any(loc => loc.name == m.Name));
        //if (close.Any())
        //{
        //    var loc = close.First();
        //    var seed = loc.seed == -1 ? Random.Range(0, 32000) : loc.seed;
        //    NetworkHelper.Instance.SpawnLocation(loc.Name, seed);
        //    //SpawnedLocations.Add(loc);
        //}
        foreach(var loc in NetworkHelper.Instance.MyLocations.Where((m) => Vector3.Distance(transform.position, m.Position) < 200 && !NetworkHelper.Instance.SpawnedInstantiatedLocations.Any(loc => loc.name == m.Name)))
        {
            //var loc = close.First();
            var seed = loc.seed == -1 ? Random.Range(0, 32000) : loc.seed;
            NetworkHelper.Instance.SpawnLocation(loc.Name, seed);
            //SpawnedLocations.Add(loc);
        }
    }

    public void DestroyLevel()
    {
        Destroy(gameObject);
    }
}
