using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LocationStation : Location
{
    public TileSet TileSet;

    public int Size = 50;
    public int TileSize = 1;
    public int Splits = 5;
    public int HalfCorridorSize = 2;
    public int MinRoomSize = 8;

    public int[,] Tiles;

    public override InstantiatedLocation SpawnLocation (Transform owner, int _seed)
    {
        seed = _seed;
        var ret = owner.GetComponent<InstantiatedLocation>();
        if (ret == null)
            ret = owner.gameObject.AddComponent<InstantiatedLocation>();

        ret.TargetLocation = this;

        var spawner = new StationSpawner();
        Tiles = spawner.Generate(owner, seed, Size, Splits, MinRoomSize, HalfCorridorSize, TileSet);

        BestFirstSearch = new BestFirstSearch(Size, Size);

        for(int i = 0; i < Size; i++)
            for(int j = 0; j < Size; j++)
            {
                if (Tiles[i, j] != 1)
                    BestFirstSearch.AddObstacle(i,j);
            }

        return ret;
    }
}

