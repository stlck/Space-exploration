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
    }

    void Update()
    {
        if (MyAvatar.Instance.isServer)
        {
            // spawn terrain in vicinity (local only)
            var close = NetworkHelper.Instance.MyLocations.Where(m => Vector3.Distance(transform.position, m.Position) < 200 && !SpawnedLocations.Contains(m));
            if (close.Any())
            {
                var loc = close.First();
                var seed = loc.seed == -1 ? Random.Range(0, 32000) : loc.seed;
                NetworkHelper.Instance.RpcSpawnLocation(loc.Name, seed);
                SpawnedLocations.Add(loc);
            }
        }
    }
    
    public void DestroyLevel()
    {
        Destroy(gameObject);
    }
}
