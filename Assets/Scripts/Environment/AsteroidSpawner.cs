using ProceduralToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidSpawner {

    public int GeneratePercentage = 70;
    public int neighborsMin = 7;

    int[,,] map;
    int Size;
    List<Vector3> UpperRegion;

	public void Generate(int s, int seed = -1)
    {
        if (seed >= 0)
            Random.seed = seed;

        Size = s;
        map = new int[Size, Size, Size];
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    map[i, j, k] = Random.Range(0, 100) > GeneratePercentage ? 1 : 0;
                }
        
    }

    public void Smooth()
    {
        int[,,] temp = new int[Size,Size,Size];
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

    int neightborCount6(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        var ret = 0;
        ret += inBounds(x - 1, y, z) ? map[x - 1, y, z] : 0;
        ret += inBounds(x + 1, y, z) ? map[x + 1, y, z] : 0;
        ret += inBounds(x, y - 1, z) ? map[x, y - 1, z] : 0;
        ret += inBounds(x, y + 1, z) ? map[x, y + 1, z] : 0;
        ret += inBounds(x, y, z - 1) ? map[x, y, z - 1] : 0;
        ret += inBounds(x, y, z + 1) ? map[x, y, z + 1] : 0;

        return ret;
    }

    int neighborCount(int x, int y, int z)
    {
        var ret = 0;
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
                for (int k = z - 1; k <= z + 1; k++)
                {
                    if (inBounds(i, j, k))
                        ret += map[i, j, k];
                }

        return ret - 1;
    }

    bool inBounds(int i, int j, int k)
    {
        return i >= 0 && i < Size && j >= 0 && j < Size && k >= 0 && k < Size;
    }

    public void IncreaseMapSize(int newsize)
    {
        var newMap = new int[newsize, newsize, newsize];
        int dif = (newsize - Size) / 2;

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    newMap[i + dif, j + dif, k + dif] = map[i, j, k];
                }

        map = newMap;
        Size = newsize;
    }

    public void doregions()
    {
        //int[,,] all = map.Clone() as int[,,];
        //var Regions = new List<List<Vector3>>();
        UpperRegion = new List<Vector3>();

        //while(all.Length > 0)
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    if (map[i, j, k] > 0)
                    {
                        var r = new List<Vector3>();
                        singleregion(r, i, j, k);
                        if (r.Count > UpperRegion.Count)
                            UpperRegion = r;
                        //Regions.Add(r);
                    }
                }

        foreach (var r in UpperRegion)
            map[(int)r.x, (int)r.y, (int)r.z] = 1;

        //map = all;
        //UpperRegion = Regions.OrderBy(m => m.Count).Last();
    }

    void singleregion(List<Vector3> region, int x, int y, int z)
    {
        region.Add(new Vector3(x, y, z));
        map[x, y, z] = 0;
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
                for (int k = z - 1; k <= z + 1; k++)
                    if (inBounds(i, j, k) && map[i, j, k] > 0)
                        singleregion(region, i, j, k);
    }

    public void Carve(Transform owner)
    {
        //var real = Time.realtimeSinceStartup;
        var y = (int)UpperRegion.Average(m => m.y);
        var x = (int)UpperRegion.Average(m => m.x);
        var z = (int)UpperRegion.Average(m => m.z);
        var lower = new GameObject();
        var upper = new GameObject();
        var p = Vector3.up * y + Vector3.forward * z + Vector3.right * x;
        var xTo = UpperRegion.Min(m => m.x) - 4;
        var lowerRegion = new List<Vector3>();
        
        // remove all at y + 1 except walls
        var toRemove = UpperRegion.Where(m => m.y == y +1 && neightborCount6(m) == 6).ToList();
        Debug.Log("toremove at " + y + ": " + toRemove.Count + "\n region count " + UpperRegion.Count);
        foreach (var t in toRemove)
            UpperRegion.RemoveAll( (m) => m == t);
        toRemove.Clear();
        Debug.Log("toremove2 at " + y + ": " + toRemove.Count + "\n region count " + UpperRegion.Count);

        // add landing bridge
        var sub = UpperRegion.Where(m => m.y == y && m.z == z);
        var pathOut = new List<Vector3>();
        for (int i = x; i > xTo; i--)
        {
            p.x++;
            if (!sub.Contains(p))
                UpperRegion.Add(p);

            pathOut.Add(p);
        }

        // add all beneath roof and 4 down to lower region
        sub = UpperRegion.Where(m => m.y <= y + 1);
        foreach (var tile in sub)
        {
            if (tile.y > y - 3 && /*neightborCount6(tile, Region) < 6 &&*/ !pathOut.Contains(tile - Vector3.up))
                lowerRegion.Add(tile);
            toRemove.Add(tile);
        }

        foreach (var t in toRemove)
            UpperRegion.Remove(t);

        lower.transform.SetParent(owner);
        lower.transform.localPosition = Vector3.zero;
        var renderer = lower.AddComponent<MeshRenderer>();
        if (owner.GetComponent<MeshRenderer>() != null)
            renderer.material = owner.GetComponent<MeshRenderer>().material;
        //StartCoroutine(regionToMeshCoroutine(lowerRegion, lower.AddComponent<MeshFilter>()));
        regionToMesh(lowerRegion,lower.AddComponent<MeshFilter>());

        upper.transform.SetParent(owner);
        upper.transform.localPosition = Vector3.zero;
        renderer = upper.AddComponent<MeshRenderer>();
        if(owner.GetComponent<MeshRenderer>() != null)
            renderer.material = owner.GetComponent<MeshRenderer>().material;
        //StartCoroutine(regionToMeshCoroutine(Region, upper.AddComponent<MeshFilter>()));
        regionToMesh(UpperRegion,upper.AddComponent<MeshFilter>());
    }

    void regionToMesh(List<Vector3> region, MeshFilter target)
    {
        float real = Time.realtimeSinceStartup;
        MeshDraft draft = new MeshDraft();

        var size = Vector3.one;
        int blocks = 0;
        foreach (var p in region)
        {
            var e = MeshDraft.Hexahedron(1, 1, 1);
            e.Move(p);
            draft.Add(e);
            //if (blocks++ % 5 == 0)
            //    yield return new WaitForEndOfFrame();
        }

        //output = draft.ToMesh();
        target.mesh = draft.ToMesh();
        target.gameObject.name = "done";
    }
}
