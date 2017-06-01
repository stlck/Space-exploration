using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LocationAsteroid : Location
{
    public int TileSize = 1;
    public List<int> SizeArray = new List<int>();

    public override InstantiatedLocation SpawnLocation (Transform owner, int _seed)
    {
        seed = _seed;
        if (seed >= 0)
            UnityEngine.Random.InitState(seed);

        var ret = owner.gameObject.AddComponent<InstantiatedAsteroid>();
        ret.TargetLocation = this;
        ret.transform.position = Position;

        var spawner = new AsteroidSpawnerNonCubed();
        var map = spawner.GetVoxelMap(SizeArray, TileSize, owner, seed);
        ret.Spawn(map, SizeArray);

        return ret;
    }

   
}
