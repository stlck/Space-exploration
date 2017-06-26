using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LocationAsteroid : Location
{
    public int TileSize = 1;
    public List<int> SizeArray = new List<int>();
    public float AsteroidFieldSize = 60f;

    public override InstantiatedLocation SpawnLocation (Transform owner, int _seed)
    {
        seed = _seed;
        if (seed >= 0)
            UnityEngine.Random.InitState(seed);

        var ret = owner.gameObject.AddComponent<InstantiatedAsteroid>();
        ret.TargetLocation = this;
        ret.transform.position = Position;

        var spawner = new AsteroidSpawnerNonCubed();
        var asteroidCount = Random.Range(1, 5);
        var positions = getPositionArray(asteroidCount);
        for(int i = 0; i < asteroidCount; i++)
        {
            if (SizeArray.Count == 0)
            {
                SizeArray.Add(Random.Range(4, 10));
                SizeArray.Add(Random.Range(10, 16));
            }
            var map = spawner.GetVoxelMap(SizeArray, TileSize, owner, seed);
            //ret.AddToVoxels(map, SizeArray, positions[i]);
            //ret.Spawn(map, SizeArray, positions[i]);
            ret.AddToSubVoxels(map, SizeArray, positions[i]);
        }
        //ret.Spawn();
        ret.FinishSpawn();
        return ret;
    }

    Vector3[] getPositionArray(int count)
    {
        Vector3[] ret = new Vector3[count];

        for(int i = 0; i < count; i++)
        {
            ret[i] = Random.insideUnitSphere * AsteroidFieldSize/3f + Vector3.one * AsteroidFieldSize/2f;
            ret[i].y = 20;
        }
        return ret;
    }
}
