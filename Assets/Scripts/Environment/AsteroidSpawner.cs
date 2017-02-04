using ProceduralToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AsteroidSpawner {

    public int GeneratePercentage = 70;
    public int neighborsMin = 7;
    public int TileSize = 1;
    LocationTileSet tileSet;

    int[,,] map;
    int Size;
    List<Vector3> UpperRegion;

    public void DoAll(List<int> SizeArray, int TileSize, Transform owner, int seed = -1, TileSet _tileSet = TileSet.BlackAsteroid)
    {
        if (seed >= 0)
            Random.InitState(seed);

        this.tileSet = Resources.LoadAll<LocationTileSet>("TileSets/" + _tileSet.ToString())[0];
        this.TileSize = TileSize;

        Generate(SizeArray[0]);
        for (int i = 1; i < SizeArray.Count; i++)
        {
            Smooth();
            IncreaseMapSize(SizeArray[i]);
            Smooth();
        }
        doregions();
        Carve2(owner);

        //Random.state = new Random.State();
    }

    public void Generate(int s, int seed = -1)
    {
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

    int neightborCount6(int x, int y, int z)
    {
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
        alignMapToRegion();
    }

    void alignMapToRegion()
    {
        map = new int[Size, Size, Size];
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
    void Carve2(Transform owner)
    {
        var y = (int)UpperRegion.Average(m => m.y);
        var x = (int)UpperRegion.Average(m => m.x);
        var z = (int)UpperRegion.Average(m => m.z);
        var lower = new GameObject();
        var upper = new GameObject();
        //var p = Vector3.up * y + Vector3.forward * z + Vector3.right * x;
        //var xMin = UpperRegion.Min(m => m.x);
        //var xTo =  xMin - 4;
        var xEntrance = 0;

        lower.transform.SetParent(owner);
        lower.name = "lower region";

        upper.transform.SetParent(owner);
        upper.name = "upper region";

        // add bridge
        for (int i = 0; i < Size; i++) {
            // landing bridge
            if (map[i, y + 1, z] == 1)
            {
                xEntrance = i;
                // make a hole into the asteroid here
                for (int j = xEntrance - 4; j < xEntrance; j++)
                    if(inBounds(j,y, z))
                        map[j, y, z] = 1;

                break;
            }
        }

        // lower region
        for (int i = 0; i < Size; i++)
            for (int j = y-4; j <= y +1; j++)
                for (int k = 0; k < Size; k++)
                { 
                    // show where not surrounded by neighbors and not where player is
                    if ((map[i,j,k] > 0 && (neightborCount6(i,j,k) < 6 || 
                        j == y && neightborCount6(i,j,k) == 6) && !(i == xEntrance && j == y +1 && k == z)) /*|| i <= x && i >= xTo && j == y && k == z*/)
                    {
                        addTileToWorld(i, j, k, lower.transform);
                    }
                }
        for (int i = 0; i < Size; i++)
            for (int j = y + 2; j < Size; j++)
                for (int k = 0; k < Size; k++)
                { // upper region
                    if (map[i,j,k] > 0 && neightborCount6(i, j, k) < 6)
                        addTileToWorld(i, j, k, upper.transform);
                }

        
        lower.transform.localPosition = Vector3.down * y;
        lower.layer = LayerMask.NameToLayer("Ship");
        upper.transform.localPosition = Vector3.down * y;
        upper.layer = LayerMask.NameToLayer("ShipTop");
    }

    void addTileToWorld(int x, int y, int z, Transform parent)
    {
        MonoBehaviour.Instantiate(tileSet.GroundTiles[LocationTileSet.GetRandom()], new Vector3(x, y, z), Quaternion.identity, parent);
    }
}
