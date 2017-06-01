using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInstancedIndirect : MonoBehaviour {

    public static Dictionary<Material, DrawInstancedIndirect> DrawDictionary = new Dictionary<Material, DrawInstancedIndirect>();
    public int instanceCount = 100000;
    public Mesh instanceMesh;
    public Material instanceMaterial;

    private int cachedInstanceCount = -1;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private List<Vector3> positionArray;
    private int layer;
    public bool markUpdate = false;

    void Start ()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);

        UpdateStationBuffers();
    }
    static void init(Material targetMaterial, int layer)
    {
        DrawDictionary.Add(targetMaterial, Instantiate(Resources.Load<DrawInstancedIndirect>("DrawIndirect")));
        
        DrawDictionary[targetMaterial].positionArray = new List<Vector3>();
        DrawDictionary[targetMaterial].instanceMaterial = targetMaterial;
        DrawDictionary[targetMaterial].layer = layer;
    }

    public static void AddToDraw(Material targetMaterial, int x, int y, int z, int layer)
    {
        if(!DrawDictionary.ContainsKey(targetMaterial))
        {
            init(targetMaterial, layer);
        }

        var pos = new Vector3(x, y, z);
        if (!DrawDictionary[targetMaterial].positionArray.Contains(pos))
        {
            DrawDictionary[targetMaterial].positionArray.Add(pos);
            DrawDictionary[targetMaterial].markUpdate = true;
        }
    }

    public static void AddToDraw(Material targetMaterial, Vector3 position, int layer)
    {
        if (!DrawDictionary.ContainsKey(targetMaterial))
        {
            init(targetMaterial, layer);
        }
        DrawDictionary[targetMaterial].positionArray.Add(position);
            DrawDictionary[targetMaterial].markUpdate = true;
    }

    public static void AddToDraw(Material targetMaterial, List<Vector3> positions, int layer)
    {
        if (!DrawDictionary.ContainsKey(targetMaterial))
        {
            init(targetMaterial, layer);
        }

        DrawDictionary[targetMaterial].positionArray.AddRange(positions);
            DrawDictionary[targetMaterial].markUpdate = true;
    }

    public static void RemoveFromDraw(Material targetMaterial, Vector3 position)
    {
        Debug.Log("REMOVE " + position);
        if (DrawDictionary.ContainsKey(targetMaterial))
        {
            Debug.Log("REMOVAL SUCCESSFULL");
            DrawDictionary[targetMaterial].positionArray.Remove(position);
            DrawDictionary[targetMaterial].markUpdate = true;
        }
    }
    public static void RemoveAll()
    {
        foreach (var d in DrawDictionary.Values)
        {
            Destroy(d.gameObject);
        }
        DrawDictionary.Clear();
    }
    public static void RemoveFromDraw(Material targetMaterial, int x, int y, int z)
    {
        if (DrawDictionary.ContainsKey(targetMaterial))
        {
            var pos = new Vector3(x, y, z);
            DrawDictionary[targetMaterial].positionArray.Remove(pos);
            DrawDictionary[targetMaterial].markUpdate = true;
        }
    }

    public static void RemoveFromDraw(Material targetMaterial, List<Vector3> positions)
    {
        if (DrawDictionary.ContainsKey(targetMaterial))
        {
            foreach(var v in positions)
                DrawDictionary[targetMaterial].positionArray.Remove(v);
            DrawDictionary[targetMaterial].markUpdate = true;
        }
    }

    void Update ()
    {
        if (markUpdate)
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
        instanceCount = positionArray.Count;

        positionBuffer = new ComputeBuffer(instanceCount, 16);
        Vector4[] positions = new Vector4[instanceCount];

        //foreach(var p in positionArray)
        for (int i = 0; i < positionArray.Count; i++)
            positions[i] = new Vector4(positionArray[i].x, positionArray[i].y, positionArray[i].z, 1);

        positionBuffer.SetData(positions);
        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);

        // indirect args
        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);

        cachedInstanceCount = instanceCount;
        markUpdate = false;
    }

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
