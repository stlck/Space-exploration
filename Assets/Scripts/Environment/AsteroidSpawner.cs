using ProceduralToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidSpawner {

    public int GeneratePercentage = 70;
    public int neighborsMin = 7;
    public int TileSize = 1;

    int[,,] map;
    int Size;
    List<Vector3> UpperRegion;

    public void DoAll(List<int> SizeArray, int TileSize, Transform owner, int seed = -1)
    {
        this.TileSize = TileSize;
        Generate(SizeArray[0]);
        for (int i = 1; i < SizeArray.Count; i++)
        {
            Smooth();
            IncreaseMapSize(SizeArray[i]);
            Smooth();
        }

        doregions();
        Carve(owner);
    }

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
        UpperRegion = new List<Vector3>();

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
                    }
                }

        foreach (var r in UpperRegion)
            map[(int)r.x, (int)r.y, (int)r.z] = 1;
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
        var toRemove = UpperRegion.Where(m => m.y == y +1 && neightborCount6(m) == 6 || m.y < y -3).ToList();

        foreach (var t in toRemove)
            UpperRegion.RemoveAll( (m) => m == t);
        toRemove.Clear();

        lowerRegion.AddRange(UpperRegion.Where(u => u.y <= y + 1));
        lowerRegion.ForEach(u => UpperRegion.Remove(u));

        // add landing bridge
        var sub = lowerRegion.Where(m => m.y == y && m.z == z);
        var pathOut = new List<Vector3>();
        for (int i = x; i > xTo; i--)
        {
            p.x++;
            if (!sub.Contains(p))
                lowerRegion.Add(p);

            pathOut.Add(p);
        }
        foreach (var t in pathOut)
            if (lowerRegion.Contains(t + Vector3.up))
                lowerRegion.Remove(t + Vector3.up);

        var baseMove = new Vector3(-x, -y, -z);

        // lower region
        lower.transform.SetParent(owner);
        lower.transform.localPosition = Vector3.zero;
        var renderer = lower.AddComponent<MeshRenderer>();
        regionToMesh(lowerRegion,lower.AddComponent<MeshFilter>(), baseMove);
        lower.name = "lower region";
        lower.layer = LayerMask.NameToLayer("Ship");
        lower.AddComponent<MeshCollider>();

        // upper region
        upper.transform.SetParent(owner);
        upper.transform.localPosition = Vector3.zero;
        renderer = upper.AddComponent<MeshRenderer>();
        regionToMesh(UpperRegion,upper.AddComponent<MeshFilter>(), baseMove);
        upper.name = "upper region";
        upper.layer = LayerMask.NameToLayer("ShipTop");
    }

    void regionToMesh(List<Vector3> region, MeshFilter target, Vector3 moveOffset)
    {
        float real = Time.realtimeSinceStartup;
        MeshDraft draft = new MeshDraft();

        var size = Vector3.one;
        foreach (var p in region)
        {
            var e = MeshDraft.Hexahedron(TileSize, TileSize, TileSize);
            e.Move((p + moveOffset) * TileSize + Vector3.down * TileSize/2);
            draft.Add(e);
        }

        target.mesh = draft.ToMesh();
    }
}
