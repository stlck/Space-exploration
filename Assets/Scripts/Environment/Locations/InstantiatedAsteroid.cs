using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InstantiatedAsteroid : InstantiatedLocation {

    public IsosurfaceMesh isosurface;
    public int[,,] CurrentMap;
    public Duplicate CubeCollider;
    List<int> SizeArray;
    LocationTileSet set;

    private void Awake()
    {
    }

    public override void BlockHit(int VoxelX, int VoxelY, int VoxelZ)
    {
        isosurface.isosurface.Data[VoxelX, VoxelY, VoxelZ] = 0;
        isosurface.isosurface.BuildData(ref isosurface.runtimeMesh);
    }

    public void Spawn(int[,,] map, List<int> sizes, TileSet tileSet = TileSet.BlackAsteroid)
    {
        CurrentMap = map;
        SizeArray = sizes;
        set = Resources.LoadAll<LocationTileSet>("TileSets/" + tileSet.ToString())[0];
        CubeCollider = Resources.Load<Duplicate>("BaseBlock");
        addMesh();
    }

    void addMesh( )
    {
        var voxelMap = new float[SizeArray[SizeArray.Count - 1], SizeArray[SizeArray.Count - 1], SizeArray[SizeArray.Count - 1]];
        for (int i = 0; i < SizeArray[SizeArray.Count - 1]; i++)
            for (int j = 0; j < SizeArray[SizeArray.Count - 1]; j++)
                for (int k = 0; k < SizeArray[SizeArray.Count - 1]; k++)
                {
                    voxelMap[i, j, k] = CurrentMap[i, j, k];
                    if (CurrentMap[i, j, k] == 1)
                    {
                        var coll = Instantiate(CubeCollider, transform);
                        coll.transform.localPosition = new Vector3(i, j, k);
                        coll.Owner = this;
                        coll.X = i;
                        coll.Y = j;
                        coll.Z = k;
                    }
                }

        var target = new GameObject("IsoMesh");
        target.transform.SetParent(transform);
        target.transform.localPosition = Vector3.zero;
        target.AddComponent<MeshFilter>();
        target.AddComponent<MeshRenderer>().sharedMaterial = set.GroundTiles.GetRandom().GetComponent<InstanceMe>().Material;
        target.transform.parent.position += Vector3.down * SizeArray[SizeArray.Count - 1] / 2f;

        isosurface = target.AddComponent<IsosurfaceMesh>();
        isosurface.isosurface = new Isosurface.Isosurface3D();
        isosurface.isosurface.Size = new Mathx.Vector3Int(SizeArray[SizeArray.Count - 1], SizeArray[SizeArray.Count - 1], SizeArray[SizeArray.Count - 1]);
        isosurface.isosurface.Data = voxelMap;
        isosurface.isosurface.BuildData(ref isosurface.runtimeMesh);
    }
}
