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
    public bool HasLights = false;

    public override InstantiatedLocation SpawnLocation (Transform owner, int _seed/*, int Size = 50, int TileSize = 1*/)
    {
        seed = _seed;
        if (seed >= 0)
            UnityEngine.Random.InitState(seed);

        var ret = owner.GetComponent<InstantiatedLocation>();
        if (ret == null)
            ret = owner.gameObject.AddComponent<InstantiatedLocation>();

        ret.TargetLocation = this;

        var spawner = new StationSpawner();
        Tiles = spawner.Generate(owner, seed, Size, Splits, MinRoomSize, HalfCorridorSize, TileSet, false);
        //ret.TileMap = spawner.tileMap;
        //ret.TileSet = TileSet;
        //ret.Size = Size;
        Rooms = spawner.Rooms;

        BestFirstSearch = new BestFirstSearch(Size, Size);

        for(int i = 0; i < Size; i++)
            for(int j = 0; j < Size; j++)
            {
                if (Tiles[i, j] != 1)
                    BestFirstSearch.AddObstacle(i,j);
            }

        return ret;
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

