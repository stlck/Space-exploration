using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInstancedIndirect : MonoBehaviour {

    public static Dictionary<Material, DrawInstancedIndirect> DrawDictionary = new Dictionary<Material, DrawInstancedIndirect>();
    public int instanceCount = 100000;
    private int cachedInstanceCount = -1;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    StationSpawner stationSpawner = new StationSpawner();
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    public Mesh instanceMesh;
    public Material instanceMaterial;
    public int StationSize = 100;
    public GameObject Cube;
    int[,] tilemap;
    Vector3[] positionArray;
    Vector3 position;
    int layer;
    void Start ()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        UpdateStationBuffers();
    }

    public static void AddTileMap(Material t, int layer, int[,] tiles, Vector3 position)
    {
        if(!DrawDictionary.ContainsKey(t))
        {
            var draw = Resources.Load<DrawInstancedIndirect>("base indirect draw");
            draw.instanceMaterial = t;

            DrawDictionary.Add(t, draw);
        }

        DrawDictionary[t].tilemap = tiles;
        DrawDictionary[t].position = position;
        DrawDictionary[t].positionArray = new Vector3[100];
        DrawDictionary[t].layer = layer;
    }

    public static void AddPosition(Material t, int layer, Vector3 position)
    {
        if (!DrawDictionary.ContainsKey(t))
        {
            var draw = Resources.Load<DrawInstancedIndirect>("base indirect draw");
            draw.instanceMaterial = t;
            
            DrawDictionary.Add(t, draw);
            DrawDictionary[t].positionArray = new Vector3[20000];
        }

        //DrawDictionary[t].positionArray 
        DrawDictionary[t].layer = layer;
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            UpdateStationBuffers();

        // Render
        Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), argsBuffer,0,null, UnityEngine.Rendering.ShadowCastingMode.On,true,layer);
    }

    void UpdateStationBuffers()
    {
        // positions
        if (positionBuffer != null)
            positionBuffer.Release();

        //var map = stationSpawner.GenerateIndirect(positionArray.Length, 10, 20, 3);
        instanceCount = positionArray.Length;

        positionBuffer = new ComputeBuffer(instanceCount, 16);
        Vector4[] positions = new Vector4[instanceCount];

        //for (int i = 0; i < StationSize; i++)
        //    for (int j = 0; j < StationSize; j++)
        //        //positions[i * stationsize + j] = new Vector4(i, 1, j, 1);
        //{ 
        //    if (map[i, j] > 0)
        //    {
        //        positions[i + j * StationSize] = new Vector4(i, 0, j, 1);
        //        Instantiate(Cube, new Vector3(i, 0, j), Quaternion.identity);
        //    }
        //}

        //foreach(var p in positionArray)
        for (int i = 0; i < positionArray.Length; i++)
            positions[i] = new Vector4(positionArray[i].x, positionArray[i].y, positionArray[i].z, 1);

        positionBuffer.SetData(positions);
        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);

        // indirect args
        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
    }

   /* void UpdateBuffers ()
    {
        // positions
        if (positionBuffer != null)
            positionBuffer.Release();

        positionBuffer = new ComputeBuffer(instanceCount, 16);
        Vector4[] positions = new Vector4[instanceCount];

        for (int i = 0; i < instanceCount; i++)
        {
            float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
            float distance = Random.Range(20.0f, 100.0f);
            float height = Random.Range(-2.0f, 2.0f);
            float size = Random.Range(0.05f, 0.25f);
            positions[i] = new Vector4(Mathf.Sin(angle) * distance, height, Mathf.Cos(angle) * distance, size);

        }

        positionBuffer.SetData(positions);
        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);

        // indirect args
        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
    }*/

    void OnDisable ()
    {
        if (positionBuffer != null)
            positionBuffer.Release();

        positionBuffer = null;

        if (argsBuffer != null)
            argsBuffer.Release();

        argsBuffer = null;
    }
}
/*
    public struct DrawStructContainer
    {
        //Graphics.DrawMeshInstancedIndirect

        public Material TargetMaterial;
        public Transform[] ToDraw;
        public int Layer;
        public int Count;
        public bool Updated;
        public Matrix4x4
    }*/
