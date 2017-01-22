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

        if(GUILayout.Button("DO MESH"))
            CreateAsMesh();

        if(GUILayout.Button("DO REGIONS"))
            doregions();

        if (GUILayout.Button("Mesh REgion"))
            regionsToMesh();

        if (GUILayout.Button("carve"))
            carve();

        if(GUILayout.Button("Do ALl"))
        {

            Generate();
            Smooth();
            Smooth();
            Smooth();
            doregions();
            regionsToMesh();
            carve();
        }

        ShowGizmos = GUILayout.Toggle(ShowGizmos, "Show gizmos");
    }

    void carve()
    {
        var y = (int)Region.Average(m => m.y);
        var x = (int)Region.Average(m => m.x);
        var z = (int)Region.Average(m => m.z);
        var c = new GameObject();
        MeshDraft interior = new MeshDraft();
        for (int i = x; i > Region.Min(m => m.x) -4; i--)
        {
            //if(!inBounds(i+2, y,z) || map[i + 2, y, z] > 0)
            { 
                var e = MeshDraft.Hexahedron(1, 1, 1);
                e.Move(Vector3.up * y + Vector3.forward * z + Vector3.right * i);
                interior.Add(e);
            }
            //interior.Add(MeshDraft.Quad(Vector3.up * y + Vector3.forward * z + Vector3.right * i, Vector3.one, Vector3.one));
        }
        output = interior.ToMesh();
        c.transform.SetParent(transform);
        c.transform.localPosition = Vector3.zero;
        c.AddComponent<MeshFilter>().mesh = output;
        c.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
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
    }

    void singleregion(List<Vector3> region, int x, int y, int z)
    {
        //region.Add()
        //region[i, j, k] = 1;
        region.Add(new Vector3(x, y, z));
        map[x, y, z] = 0;
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
                for (int k = z - 1; k <= z + 1; k++)
                    if (inBounds(i,j,k) && map[i, j, k] > 0)
                        singleregion(region, i, j, k);                  
    }

    void regionsToMesh()
    {
        MeshDraft draft = new MeshDraft();
        Region = Regions.OrderBy(m => m.Count).Last();

        foreach (var p in Region.Where(r => neightborCount6(r) < 6))
        {
            {
                var e = MeshDraft.Hexahedron(1, 1, 1);
                e.Move(p);
                draft.Add(e);
            }
        }

        output = draft.ToMesh();
        GetComponent<MeshFilter>().mesh = output;
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

    void Generate()
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    map[i, j, k] = Random.Range(0, 100) > GeneratePercentage ? 1 : 0;
                }
    }

    void Smooth()
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    var nCount = neighborCount(i, j, k);
                    if (nCount > neighborsMin)
                        map[i, j, k] = 1;
                    else if (nCount < neighborsMin)
                        map[i, j, k] = 0;
                }
    }

    int neightborCount6(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;
        /*
        var ret = 0;
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
                for (int k = z - 1; k <= z + 1; k++)
                {
                    if (inBounds(i, j, k))//i >= 0 && i < Size && j >= 0 && j < Size && k >= 0 && k < Size)
                        ret += map[i, j, k];
                }
        */
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
                    if (inBounds(i,j,k))//i >= 0 && i < Size && j >= 0 && j < Size && k >= 0 && k < Size)
                        ret += map[i, j, k];
                }

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
