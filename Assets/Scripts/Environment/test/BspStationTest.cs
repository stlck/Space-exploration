using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BspStationTest : MonoBehaviour {

    private float timer = 0;
    private GameObject parent;

    public bool autoCreate = false;
    public int iterations = 2;
    public int size = 70;
    public int minRoomSize = 8;
    public int halfCorridorSize = 1;
    public LocationTileSet _tileset;
    public int seed = -1;
    List<int> sizes;
    public bool doStations = true;
    public Material AsteroidMaterial;
    public float Target = .5f;

    void Awake()
    {
        sizes = new List<int>() { 14, 18 };
    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if( Input.GetKeyDown(KeyCode.Space) || autoCreate && timer > .5f)
        {
            timer = 0f;
            if (doStations)
                generateStation();
            else
                generateAsteroid();
        }

	}

    void OnGUI()
    {
        autoCreate = GUILayout.Toggle(autoCreate, "Auto");
        doStations = GUILayout.Toggle(doStations, doStations ? "stations" : "Asteroids");

        GUILayout.Label("Size : " + size);
        size = (int)GUILayout.HorizontalSlider((float)size, 0f, 100f);

        if (doStations)
        {
            GUILayout.Label("Splits : " + iterations);
            iterations = (int)GUILayout.HorizontalSlider((float)iterations, 1f, 8f);

            GUILayout.Label("Min Room Size : " + minRoomSize);
            minRoomSize = (int)GUILayout.HorizontalSlider((float)minRoomSize, 4f, 20f);

            GUILayout.Label("Half corridor size : " + halfCorridorSize);
            halfCorridorSize = (int)GUILayout.HorizontalSlider((float)halfCorridorSize, 1f, 4f);
        }
        else
        {
            GUILayout.Label("asteroid size array");
            for (int i = 0; i < sizes.Count; i++)
                sizes[i] = (int) GUILayout.HorizontalSlider((float)sizes[i], (float)(i == 0 ? 10 : sizes[i - 1]), (float)(i == sizes.Count - 1 ? 30 : sizes[i + 1]));
        }
    }
    public IsosurfaceMesh isosurface;
    void generateStation()
    {
        if (parent != null)
            Destroy(parent);

        parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        StationSpawner spawner = new StationSpawner();
        spawner.Tester = this;

        var map = spawner.Generate(parent.transform, seed, size, iterations, minRoomSize, halfCorridorSize, TileSet.BlueStation, false);
        var height = 10;
        var voxelMap = spawner.ToVoxelMap(height);

        for (int x = 0; x < voxelMap.GetLength(0); x++)
            for (int y = 0; y < voxelMap.GetLength(1); y++)
                for (int z = 0; z < voxelMap.GetLength(2); z++)
                    if(voxelMap[x,y,z] > 0)
                        CoroutineSpawner.Instance.Enqueue(_tileset.GroundTiles[voxelMap[x,y,z]-1], Vector3.right * x + Vector3.forward * z + Vector3.up * y, Quaternion.identity, parent.transform);
    }

    void generateAsteroid()
    {
        if (parent != null)
            Destroy(parent);

        if (parent == null)
        {
            parent = new GameObject();
            parent.AddComponent<MeshFilter>();
            parent.AddComponent<MeshRenderer>().sharedMaterial = AsteroidMaterial;
        }

        AsteroidSpawnerNonCubed spawner = new AsteroidSpawnerNonCubed();

        currentMap = spawner.GetVoxelMap(sizes, 1, parent.transform, seed);
        var voxelMap = new float[sizes[sizes.Count - 1], sizes[sizes.Count - 1], sizes[sizes.Count - 1]];
        for (int i = 0; i < sizes[sizes.Count-1]; i++)
            for (int j = 0; j < sizes[sizes.Count - 1]; j++)
                for (int k = 0; k < sizes[sizes.Count - 1]; k++)
                {
                    voxelMap[i, j, k] = currentMap[i, j, k];
                    if (currentMap[i,j,k] == 1)
                    {
                        var coll = Instantiate(CubeCollider, parent.transform);
                        coll.transform.localPosition = new Vector3(i, j, k);
                        coll.VoxelX = i;
                        coll.VoxelY = j;
                        coll.VoxelZ = k;
                    }
                }
        //mapToMesh(parent.transform, currentMap, sizes[sizes.Count - 1], sizes[sizes.Count - 1], sizes[sizes.Count - 1]);
        isosurface.isosurface.Size = new Mathx.Vector3Int(sizes[sizes.Count - 1], sizes[sizes.Count - 1], sizes[sizes.Count - 1]);
        isosurface.isosurface.Data = voxelMap;
        isosurface.isosurface.BuildData(ref isosurface.runtimeMesh);
    }

    public VoxelCollider CubeCollider;
    int[,,] currentMap;
    public void ColliderHit(int VoxelX, int VoxelY, int VoxelZ)
    {
        isosurface.isosurface.Data[VoxelX, VoxelY, VoxelZ] = 0;
        isosurface.isosurface.BuildData(ref isosurface.runtimeMesh);
    }

}