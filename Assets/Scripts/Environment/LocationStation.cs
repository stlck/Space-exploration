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
        var _tiles = MapTiles;// BlockGenerator.GenerateMap(Width, Height);

        if(Type == LocationTypes.Asteroid)
        {
            addGroundAsMesh(GroundTiles, owner);
        }
        
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
            {
                var tile = _tiles[i, j];

                var p = Position + Vector3.right * i * TileSize + Vector3.forward * j * TileSize;// + Vector3.down / 2;

                if (GroundTiles[tile] != null)
                {
                    if (tile == 3)
                    {
                        wallAt(p, GroundTiles[tile], owner, WallHeight);
                        if (i + 1 < Size && _tiles[i + 1, j] == 3)
                            for (int temp = 0; temp < TileSize - 1; temp++)
                                wallAt(p + Vector3.right * (temp + 1), GroundTiles[tile], owner, WallHeight);
                        if (j + 1 < Size && _tiles[i, j + 1] == 3)
                            for (int temp = 0; temp < TileSize - 1; temp++)
                                wallAt(p + Vector3.forward * (temp + 1), GroundTiles[tile], owner, WallHeight);
                        tile = 2;
                    }

                    if(Type == LocationTypes.SpaceStation)
                    { 
                        var t = Instantiate(
                            GroundTiles[tile],
                            p,
                            Quaternion.identity, owner) as Transform;

                        t.localScale = s;
                    }
                }
                else if(neighborCount(_tiles, i, j) > 0)
                {
                    
                    var t = Instantiate(
                        GroundTiles[0],
                        p ,
                        Quaternion.identity, owner) as Transform;

                    var scale = s;
                    scale.y = WallHeight;
                    t.localScale = scale;
                    t.localPosition = t.localPosition + Vector3.up * TileSize / 2;
                    t.gameObject.layer = LayerMask.NameToLayer("ShipTop");
                }
            }
    }

    int neighborCount(int[,] _tiles, int x, int z)
    {
        var ret = 0;
        for(int i = x- 1; i <= x +1; i++)
            for(int j = z -1; j <= z +1; j++)
            {
                if (i>= 0 && j>= 0 && i < Size && j < Size && _tiles[i, j] != 1)
                    ret++;
            }

        return ret;
    }

    void addGroundAsMesh(List<Transform> GroundTiles, Transform owner)
    {
        var mesh = new MeshGenerator();
        mesh.Test = this;
        mesh.newGen();
        
        var ground = new GameObject();
        ground.AddComponent<MeshFilter>();
        ground.AddComponent<MeshRenderer>().material = GroundTiles[0].GetComponent<MeshRenderer>().sharedMaterial;

        ground.transform.SetParent(owner);
        ground.transform.localPosition = Vector3.zero;
        ground.GetComponent<MeshFilter>().mesh = mesh.output;
        ground.AddComponent<MeshCollider>().sharedMesh = mesh.output;
    }

    void wallAt(Vector3 p, Transform t, Transform owner, int WallHeight)
    {
        for (int k = 0; k < WallHeight; k++)
        {
            Instantiate(t, p + Vector3.up * (k +1), Quaternion.identity, owner);
        }
    }
}

