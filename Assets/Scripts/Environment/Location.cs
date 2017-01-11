using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu()]
public class Location : ScriptableObject {

    public string Name = "";
    public Vector3 Position;
    public int Size = 50;
    public int TileSize = 1;
    public LocationTypes Type;
    public LocationStandings Standing;

    public int[,] MapTiles
    {
        get
        {
            var ret = new int[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    ret[i, j] = tiles[(i + j * Size)];
                }
            }
            return ret;
        }
        set
        {
            tiles = new int[(int)(Size * Size)];
            for (int i = 0; i < (int)Size; i++)
            {
                for (int j = 0; j < (int)Size; j++)
                {
                    tiles[i + j* Size]= value[i, j];
                }
            }
        }
    }
    public int[] tiles;
    [ContextMenuItem("FillMap", "FillMap")]
    [ContextMenuItem("OutputMap", "OutputMap")]
    public ProceduralBlocks BlockGenerator;

    public void FillMap()
    {
        MapTiles = BlockGenerator.GenerateMap(Size, Size);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public void OutputMap()
    {
        string output = "";

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Debug.Log(MapTiles[i, j]);
                output += MapTiles[i, j];
            }
            output += "\n";
        }

        Debug.Log(output);
    }
}

public enum LocationTypes
{
    Asteroid,
    SpaceEncounter,
    SpaceStation,
}

public enum LocationStandings
{
    Hostile,
    Friendly,
    Abandoned,
}