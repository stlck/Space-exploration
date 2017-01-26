using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit;using System.Linq;

public class CATest : MonoBehaviour
{
    public float GeneratePercentage = 60;
    public int Size = 15;
    public int neighborsMin = 3;

    public bool ShowGizmos = false;

    public List<List<Vector3>> Regions;
    public List<Vector3> Region;
    int[,,] map;
    Mesh output;

    // Use this for initialization
    void Start()
    {
        map = new int[Size, Size, Size];
        output = new Mesh();
    }

    void OnGUI()
    {
        if(GUILayout.Button("DO CA"))
            Generate();
        
        if (GUILayout.Button("SMOOTH CA"))
            Smooth();
        if (GUILayout.Button("INCREASE SIZE"))
            IncreaseMapSize(Size * 2);

        if (GUILayout.Button("DO MESH"))
            CreateAsMesh();

        if(GUILayout.Button("DO REGIONS"))
            doregions();

        //if (GUILayout.Button("Mesh REgion"))
        //    regionsToMesh();

        if (GUILayout.Button("carve"))
            carve2();

        if(GUILayout.Button("Do ALl"))
        {
            float real = Time.realtimeSinceStartup;
            
            Debug.Log("Region to mesh " + (Time.realtimeSinceStartup - real));
            Generate();
        Debug.Log("1 " + (Time.realtimeSinceStartup - real));
            Smooth();
        Debug.Log("2 " + (Time.realtimeSinceStartup - real));
            Smooth();
        Debug.Log("3 " + (Time.realtimeSinceStartup - real));
            IncreaseMapSize(20);
        Debug.Log("4 " + (Time.realtimeSinceStartup - real));
            Smooth();
        Debug.Log("5 " + (Time.realtimeSinceStartup - real));
            Smooth();
        Debug.Log("6 " + (Time.realtimeSinceStartup - real));
            IncreaseMapSize(26);
        Debug.Log("7 " + (Time.realtimeSinceStartup - real));
            Smooth();
        Debug.Log("8 " + (Time.realtimeSinceStartup - real));
            doregions();

        Debug.Log("9 " + (Time.realtimeSinceStartup - real));
            carve2();
        Debug.Log("10 " + (Time.realtimeSinceStartup - real));
        }

        ShowGizmos = GUILayout.Toggle(ShowGizmos, "Show gizmos");
    }
    void carve2()
    {
        //var real = Time.realtimeSinceStartup;
        var y = (int)Region.Average(m => m.y);
        var x = (int)Region.Average(m => m.x);
        var z = (int)Region.Average(m => m.z);
        var lower = new GameObject();
        var upper = new GameObject();
        var p = Vector3.up * y + Vector3.forward * z + Vector3.right * x;
        var xTo = Region.Min(m => m.x) - 4;
        var lowerRegion = new List<Vector3>();
        
        var toRemove = Region.Where(m => m.y != y && neightborCount6(m) == 6).ToList();
        foreach (var t in toRemove)
            Region.Remove(t);
        toRemove.Clear();

        var sub = Region.Where(m => m.y == y && m.z == z);
        var pathOut = new List<Vector3>();
        for (int i = x; i > xTo; i--)
        {
            p.x++;
            if (!sub.Contains(p))
                Region.Add(p);

           pathOut.Add(p);     
        }

        sub = Region.Where(m => m.y <= y + 1);
        foreach (var tile in sub)
        {
            if (tile.y > y - 3 && /*neightborCount6(tile, Region) < 6 &&*/ !pathOut.Contains(tile - Vector3.up))
                lowerRegion.Add(tile);
            toRemove.Add(tile);
        }

        foreach (var t in toRemove)
            Region.Remove(t);
        
        lower.transform.SetParent(transform);
        lower.transform.localPosition = Vector3.zero;
        lower.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        StartCoroutine(regionToMeshCoroutine(lowerRegion, lower.AddComponent<MeshFilter>()));

        upper.transform.SetParent(transform);
        upper.transform.localPosition = Vector3.zero;
        upper.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        StartCoroutine(regionToMeshCoroutine(Region, upper.AddComponent<MeshFilter>()));
    }

    void doregions()
    {
        int[,,] all = map.Clone() as int[,,];
        Regions = new List<List<Vector3>>();

        //while(all.Length > 0)
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    if (all[i, j, k] > 0)
                    {
                        var r = new List<Vector3>();
                        singleregion(r ,i, j, k);
                        Regions.Add(r);
                    }
                }
        map = all;
        Region = Regions.OrderBy(m => m.Count).Last();
    }

    void singleregion(List<Vector3> region, int x, int y, int z)
    {
        region.Add(new Vector3(x, y, z));
        map[x, y, z] = 0;
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
                for (int k = z - 1; k <= z + 1; k++)
                    if (inBounds(i,j,k) && map[i, j, k] > 0)
                        singleregion(region, i, j, k);                  
    }

    void regionsToMesh(List<Vector3> region, MeshFilter target)
    {
        MeshDraft draft = new MeshDraft();
        
        var sub = region.Where(r => neightborCount6(r, region) < 6);
        var dir = Directions.All & ~Directions.Down;
        var size = Vector3.one;
        foreach (var p in sub )
        {
            var e = MeshDraft.Hexahedron(size,size,size, dir);
            e.Move(p);
            draft.Add(e);
        }

        target.mesh = draft.ToMesh();
    }

    IEnumerator regionToMeshCoroutine(List<Vector3> region, MeshFilter target)
    {
        float real = Time.realtimeSinceStartup;
        MeshDraft draft = new MeshDraft();

        var size = Vector3.one;
        int blocks = 0;
        foreach (var p in region)
        {
            var e = MeshDraft.Hexahedron(1,1,1);
            e.Move(p);
            draft.Add(e);
            if(blocks++ % 5 == 0)
                yield return new WaitForEndOfFrame();
        }

        //output = draft.ToMesh();
        target.mesh = draft.ToMesh();
        target.gameObject.name = "done";
    }

    void CreateAsMesh()
    {
        MeshDraft draft = new MeshDraft();
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    if(map[i,j,k] > 0)
                    {
                        var e = MeshDraft.Hexahedron(1, 1, 1);
                        e.Move(indexToPosition(i,j,k));
                        draft.Add(e);
                    }
                }

        output = draft.ToMesh();
        GetComponent<MeshFilter>().mesh = output;
    }

    Vector3 indexToPosition(int i, int j, int k)
    {
        return Vector3.right * i + Vector3.up * j + Vector3.forward * k;
    }

    void IncreaseMapSize(int newsize)
    {
        var newMap = new int[newsize, newsize, newsize];
        int dif = (newsize - Size)/2;

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    newMap[i + dif, j+ dif, k+ dif] = map[i, j, k];
                }

        map = newMap;
        Size = newsize;
    }

    void Generate()
    {
        Size = 15;
        map = new int[Size, Size, Size];
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    map[i, j, k] = Random.Range(0, 100) > GeneratePercentage ? 1 : 0;
                }
    }

    void Smooth()
    {
        int[,,] temp = new int[Size, Size, Size];
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    { 
                    var nCount = neighborCount(i, j, k);
                        if (nCount >= neighborsMin)
                            temp[i, j, k] = 1;
                        else if (nCount < neighborsMin)
                            temp[i, j, k] = 0;
                    }
                }
        map = temp;
    }
    int neightborCount6(Vector3 pos, List<Vector3> region)
    {
        var ret = 0;
        ret = region.Count(m => m == pos + Vector3.up ||
                                m == pos + Vector3.down ||
                                m == pos + Vector3.right ||
                                m == pos + Vector3.left ||
                                m == pos + Vector3.forward ||
                                m == pos + Vector3.back );

        return ret;
    }

    int neightborCount6(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        var ret = 0;
        ret += inBounds(x-1, y, z) ? map[x-1, y, z] : 0;
        ret += inBounds(x+1, y, z) ? map[x+1, y, z] : 0;
        ret += inBounds(x, y-1, z) ? map[x, y-1, z] : 0;
        ret += inBounds(x, y+1, z) ? map[x, y+1, z] : 0;
        ret += inBounds(x, y, z-1) ? map[x, y, z-1] : 0;
        ret += inBounds(x, y, z+1) ? map[x, y, z+1] : 0;

        return ret;
    }

    int neighborCount(int x, int y, int z)
    {
        var ret = 0;
        for (int i = x-1; i <= x+1; i++)
            for (int j = y-1; j <= y+1; j++)
                for (int k = z-1; k <= z+1; k++)
                {
                    if (inBounds(i,j,k))
                        ret += map[i, j, k];
                }

        //ret += inBounds(x - 1, y, z) ? map[x - 1, y, z] : 0;
        //ret += inBounds(x + 1, y, z) ? map[x + 1, y, z] : 0;
        //ret += inBounds(x, y - 1, z) ? map[x, y - 1, z] : 0;
        //ret += inBounds(x, y + 1, z) ? map[x, y + 1, z] : 0;
        //ret += inBounds(x, y, z - 1) ? map[x, y, z - 1] : 0;
        //ret += inBounds(x, y, z + 1) ? map[x, y, z + 1] : 0;

        return ret -1;
    }

    bool inBounds(int i, int j, int k)
    {
        return i >= 0 && i < Size && j >= 0 && j < Size && k >= 0 && k < Size;
    }

    public void OnDrawGizmos()
    {
        if(map != null && ShowGizmos)
        {
            for(int i = 0; i < Size;i++)
                for(int j = 0; j < Size;j++)
                    for(int k = 0; k < Size;k++)
                    {
                        if(map[i,j,k] > 0)
                            Gizmos.DrawCube(Vector3.right * i + Vector3.up* j + Vector3.forward * k, Vector3.one);
                    }
        }
    }
}
