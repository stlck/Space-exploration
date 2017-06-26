using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InstantiatedAsteroid : InstantiatedLocation {

    public IsosurfaceMesh isosurface;
    public int[,,] CurrentMap;
    public Duplicate CubeCollider;
    public DuplicateFragment EffectOnBlockDeath;
    List<int> SizeArray;
    LocationTileSet set;
    Material mat;

    private void Awake()
    {
        CurrentMap = new int[80, 50, 80];
        EffectOnBlockDeath = Resources.Load<DuplicateFragment>("TileSets/BlockDeathEffect");
    }

    public override void BlockHit(int VoxelX, int VoxelY, int VoxelZ)
    {
        var possibleAsteroids = singleAsteroids.Where(m => m.StartX < VoxelX && m.StartY < VoxelY && m.StartZ < VoxelZ && m.StartX + m.Size > VoxelX && m.StartY + m.Size > VoxelY && m.StartZ + m.Size > VoxelZ);

        if(possibleAsteroids.Any())
        {
            var hit = possibleAsteroids.First();
            if (EffectOnBlockDeath != null)
            {
                var p = new Vector3(VoxelX, VoxelY, VoxelZ) + hit.Target.position;
                var e = Instantiate(EffectOnBlockDeath, p, EffectOnBlockDeath.transform.rotation);
                //if (materialOverview[pos] != null)
                e.SetColor(mat.color);
            }
            hit.Surface.isosurface.Data[VoxelX - hit.StartX, VoxelY - hit.StartY, VoxelZ - hit.StartZ] = 0;
            //isosurface.isosurface.Data[VoxelX, VoxelY, VoxelZ] = 0;
            hit.Surface.isosurface.BuildData(ref isosurface.runtimeMesh);
            //isosurface.isosurface.BuildData(ref isosurface.runtimeMesh);
        }
    }

    // build up voxelmap
    public void AddToVoxels(int[,,] map, List<int> sizes, Vector3 offset)
    {
        SizeArray = sizes;
        int offsetX = (int)offset.x;
        int offsetY = (int)offset.y;
        int offsetZ = (int)offset.z;
        for (int i = 0; i < SizeArray[SizeArray.Count - 1]; i++)
            for (int j = 0; j < SizeArray[SizeArray.Count - 1]; j++)
                for (int k = 0; k < SizeArray[SizeArray.Count - 1]; k++)
                    CurrentMap[i + offsetX, j + offsetY, k + offsetZ] = map[i, j, k];
    }

    // add collision cubes and spawn all voxels into map
    public void Spawn( TileSet tileSet = TileSet.BlackAsteroid)
    {
        var voxelMap = addCubes();
        set = Resources.LoadAll<LocationTileSet>("TileSets/" + tileSet.ToString())[0];
        addMesh( voxelMap);
        var pos = transform.position;
        pos.y = -voxelMap.GetLength(1) / 2f - SizeArray.Last() /2f;
        transform.position = pos;
    }

    // add collision cubes
    float[,,] addCubes()
    {
        if (CubeCollider == null)
            CubeCollider = Resources.Load<Duplicate>("BaseBlock");

        var voxelMap = new float[CurrentMap.GetLength(0), CurrentMap.GetLength(1), CurrentMap.GetLength(2)];
        for (int i = 0; i < CurrentMap.GetLength(0); i++)
            for (int j = 0; j < CurrentMap.GetLength(1); j++)
                for (int k = 0; k < CurrentMap.GetLength(2); k++)
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
        return voxelMap;
    }

    // create meshes
    void addMesh( float[,,] voxels)
    {
        var target = new GameObject("IsoMesh");
        target.transform.SetParent(transform);
        target.transform.localPosition = Vector3.zero;
        target.AddComponent<MeshFilter>();
        mat = set.GroundTiles.GetRandom().GetComponent<InstanceMe>().Material;
        target.AddComponent<MeshRenderer>().sharedMaterial = mat;

        isosurface = target.AddComponent<IsosurfaceMesh>();
        isosurface.isosurface = new Isosurface.Isosurface3D();
        isosurface.isosurface.Size = new Mathx.Vector3Int(voxels.GetLength(0), voxels.GetLength(1), voxels.GetLength(2));
        //isosurface.isosurface.Size = new Mathx.Vector3Int(SizeArray[SizeArray.Count - 1], SizeArray[SizeArray.Count - 1], SizeArray[SizeArray.Count - 1]);
        isosurface.isosurface.Data = voxels;
        isosurface.isosurface.BuildData(ref isosurface.runtimeMesh);
    }

    public void FinishSpawn()
    {
        var pos = transform.position;
        pos.y =- maxSizeY / 2f;//-voxelMap.GetLength(1) / 2f - SizeArray.Last() / 2f;
        transform.position = pos;
    }

    public void AddToSubVoxels(int[,,] map, List<int> size, Vector3 offset, TileSet tileSet = TileSet.BlackAsteroid)
    {
        set = Resources.LoadAll<LocationTileSet>("TileSets/" + tileSet.ToString())[0];
        addSubCubes(map, offset);
    }

    void addSubCubes(int[,,] map, Vector3 offset)
    {
        if (CubeCollider == null)
            CubeCollider = Resources.Load<Duplicate>("BaseBlock");
        int offsetX = (int)offset.x;
        int offsetY = (int)offset.y;
        int offsetZ = (int)offset.z;


        var voxelMap = new float[map.GetLength(0), map.GetLength(1), map.GetLength(2)];
        for (int i = 0; i < map.GetLength(0); i++)
            for (int j = 0; j < map.GetLength(1); j++)
                for (int k = 0; k < map.GetLength(2); k++)
                {
                    voxelMap[i, j, k] = map[i, j, k];
                    if (map[i, j, k] == 1)
                    {
                        var coll = Instantiate(CubeCollider, transform);
                        coll.transform.localPosition = new Vector3(i, j, k) + offset;
                        coll.Owner = this;
                        coll.X = i + offsetX;
                        coll.Y = j + offsetY;
                        coll.Z = k + offsetZ;
                    }
                }
        if (maxSizeY < map.GetLength(1))
            maxSizeY = map.GetLength(1);
        var container = new asteroidContainer() { StartX = offsetX, StartY = offsetY, StartZ = offsetZ, Map = voxelMap, Size = map.GetLength(0) };

        addSubMesh(voxelMap, offset, container);
    }

    void addSubMesh(float[,,] voxels, Vector3 localPosition, asteroidContainer container)
    {
        var target = new GameObject("IsoMesh");
        target.transform.SetParent(transform);
        target.transform.localPosition = localPosition;
        target.AddComponent<MeshFilter>();
        mat = set.GroundTiles.GetRandom().GetComponent<InstanceMe>().Material;
        target.AddComponent<MeshRenderer>().sharedMaterial = mat;

        isosurface = target.AddComponent<IsosurfaceMesh>();
        isosurface.isosurface = new Isosurface.Isosurface3D();
        isosurface.isosurface.Size = new Mathx.Vector3Int(voxels.GetLength(0), voxels.GetLength(1), voxels.GetLength(2));
        isosurface.isosurface.Data = voxels;
        isosurface.isosurface.BuildData(ref isosurface.runtimeMesh);

        container.Target = target.transform;
        container.Surface = isosurface;
        singleAsteroids.Add(container);
    }

    int maxSizeY = 0;
    List<asteroidContainer> singleAsteroids = new List<asteroidContainer>();

    class asteroidContainer
    {
        public int StartX;
        public int StartY;
        public int StartZ;

        public int Size;

        public float[,,] Map;
        public Transform Target;
        public IsosurfaceMesh Surface;
    }
}
