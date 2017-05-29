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
        spawn(owner);
        return ret;
    }

    void spawn(Transform Owner)
    {
        var spawner = new AsteroidSpawnerNonCubed();

        var map = spawner.GetVoxelMap(SizeArray, TileSize, Owner, seed);
    }
}
