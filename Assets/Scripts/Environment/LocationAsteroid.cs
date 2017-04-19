using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LocationAsteroid : Location
{
    public TileSet TileSet;

    public int TileSize = 1;
    public List<int> SizeArray = new List<int>();

    public override InstantiatedLocation SpawnLocation (Transform owner, int _seed)
    {
        seed = _seed;
        if (seed >= 0)
            UnityEngine.Random.InitState(seed);

        var ret = owner.GetComponent<InstantiatedLocation>();
        if (ret == null)
            ret = owner.gameObject.AddComponent<InstantiatedLocation>();
        ret.TargetLocation = this;
        //var ret = base.SpawnLocation(owner, _seed);
        spawn(owner);
        return ret;
    }

    void spawn(Transform Owner)
    {
        var spawner = new AsteroidSpawner();
        //Debug.Log("SPAWNING WITH Percentage: " + spawner.GeneratePercentage + ". neighbors: " + spawner.neighborsMin);

        spawner.DoAll(SizeArray, TileSize, Owner, seed, TileSet);
        
        /*var set = Resources.LoadAll<LocationTileSet>("TileSets/" + TileSet.ToString())[0];
        foreach(var r in Owner.GetComponentsInChildren<MeshRenderer>())
            r.material = set.GroundTiles[0].GetComponent<MeshRenderer>().sharedMaterial;*/

    }



}
