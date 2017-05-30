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
    public List<BspCell> Rooms;
    StationSpawner spawner;

    public override InstantiatedLocation SpawnLocation (Transform owner, int _seed)
    {
        seed = _seed;
        if (seed >= 0)
            UnityEngine.Random.InitState(seed);

        var ret = owner.GetComponent<InstantiatedLocation>();
        if (ret == null)
            ret = owner.gameObject.AddComponent<InstantiatedLocation>();

        ret.TargetLocation = this;

        spawner = new StationSpawner();
        Tiles = spawner.Generate(owner, seed, Size, Splits, MinRoomSize, HalfCorridorSize, TileSet, false);
        Rooms = spawner.Rooms;
        createBlocks(owner);

        BestFirstSearch = new BestFirstSearch(Size, Size);

        for(int i = 0; i < Size; i++)
            for(int j = 0; j < Size; j++)
            {
                if (Tiles[i, j] != 1)
                    BestFirstSearch.AddObstacle(i,j);
            }

        return ret;
    }

    void createBlocks(Transform parent)
    {
        var set = Resources.LoadAll<LocationTileSet>("TileSets/" + TileSet.ToString())[0];
        var height = 10;
        var voxelMap = spawner.ToVoxelMap(height);

        for (int x = 0; x < voxelMap.GetLength(0); x++)
            for (int y = 0; y < voxelMap.GetLength(1); y++)
                for (int z = 0; z < voxelMap.GetLength(2); z++)
                    if (voxelMap[x, y, z] > 0)
                    { 
                        CoroutineSpawner.Instance.Enqueue(set.GroundTiles[voxelMap[x, y, z] - 1], Vector3.right * x + Vector3.forward * z + Vector3.up * (y-2), Quaternion.identity, parent.transform);
                    }
    }

    public override void ShowCreator ()
    {
        base.ShowCreator();

        GUILayout.Label("Size " + Size);
        Size = (int) GUILayout.VerticalSlider(Size, 20, 100);

        GUILayout.Label("TileSize " + TileSize);
        TileSize = (int)GUILayout.VerticalSlider(TileSize, 1, 3);

        GUILayout.Label("Splits " + Splits);
        Splits = (int)GUILayout.VerticalSlider(Splits, 2, 8);

        GUILayout.Label("HalfCorridorSize " + HalfCorridorSize);
        HalfCorridorSize = (int) GUILayout.VerticalSlider(HalfCorridorSize, 1, 4);

        GUILayout.Label("MinRoomSize " + MinRoomSize);
        MinRoomSize = (int) GUILayout.VerticalSlider(MinRoomSize, 4, 20);
    }
}

