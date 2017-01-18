using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LocationStation : Location
{
    //public List<Transform> GroundTiles;
    //public LocationTileSet LocationTileSet;
    public TileSet TileSet;

    public int Size = 50;
    public int TileSize = 1;
    public int WallHeight = 4;
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
                    tiles[i + j * Size] = value[i, j];
                }
            }
        }
    }

    public int[] tiles;
    [ContextMenuItem("FillMap", "FillMap")]
    public ProceduralBlocks BlockGenerator;

    public void FillMap()
    {
        MapTiles = BlockGenerator.GenerateMap(Size, Size);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    public override void SpawnLocation(Transform owner)
    {
        base.SpawnLocation(owner);

        var set = Resources.LoadAll<LocationTileSet>("TileSets/" + TileSet.ToString())[0];
        var GroundTiles = set.GroundTiles;

        var s = (Vector3.forward + Vector3.right) * TileSize + Vector3.up;
        var tiles = MapTiles;// BlockGenerator.GenerateMap(Width, Height);

        var mesh = new MeshGenerator();
        mesh.Test = this;
        mesh.newGen();

        var ground = new GameObject();
        ground.AddComponent<MeshFilter>();
        ground.AddComponent<MeshRenderer>().material = GroundTiles[0].GetComponent<MeshRenderer>().sharedMaterial;
        
        ground.transform.SetParent(owner);
        ground.transform.localPosition = Vector3.zero;
        ground.GetComponent<MeshFilter>().mesh = mesh.output;

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
            {
                var tile = tiles[i, j];

                if (GroundTiles[tile] != null)
                {
                    var p = Position + Vector3.right * i * TileSize + Vector3.forward * j * TileSize;// + Vector3.down / 2;

                    if (tile == 3)
                    {
                        wallAt(p, GroundTiles[tile], owner, WallHeight);
                        if (i + 1 < Size && tiles[i + 1, j] == 3)
                            for (int temp = 0; temp < TileSize; temp++)
                                wallAt(p + Vector3.right * (temp + 1), GroundTiles[tile], owner, WallHeight);
                        if (j + 1 < Size && tiles[i, j + 1] == 3)
                            for (int temp = 0; temp < TileSize; temp++)
                                wallAt(p + Vector3.forward * (temp + 1), GroundTiles[tile], owner, WallHeight);
                        tile = 2;
                    }

                   /* var t = Instantiate(
                        GroundTiles[tile],
                        p,
                        Quaternion.identity) as Transform;

                    t.localScale = s;
                    t.SetParent(owner);*/

                }
            }
    }

    void wallAt(Vector3 p, Transform t, Transform owner, int WallHeight)
    {
        for (int k = 0; k < WallHeight; k++)
        {
            Instantiate(t, p + Vector3.up * (k +1), Quaternion.identity, owner);
        }
    }
}

