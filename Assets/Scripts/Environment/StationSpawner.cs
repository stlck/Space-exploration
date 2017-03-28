using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationSpawner
{
    BspCell root;
    int[,] map;
    int size;
    int iterations = 2;
    int minRoomSize = 8;
    int halfCorridorSize = 1;
    Transform parent;
    TileSet TileSet;
    public BspStationTest Tester;

    public int[,] Generate(Transform _parent, int seed, int _size, int _splits = 5, int _minRoomSize = 8, int _halfCorridorSize = 1, TileSet _tileSet = TileSet.BlackAsteroid, bool buildmesh = true)
    {
        size = _size;
        map = new int[size, size];

        if (seed >= 0)
            Random.InitState(seed);

        iterations = _splits;
        minRoomSize = _minRoomSize;
        halfCorridorSize = _halfCorridorSize;
        parent = _parent;
        TileSet = _tileSet;

        doAll(buildmesh);

        return map;
    }

    void doAll(bool meshit)
    {
        var timer = Time.realtimeSinceStartup;
        Debug.Log("t1 : " + (Time.realtimeSinceStartup - timer));
        root = new BspCell(size / 2, size / 2, size, size);
        var cite = iterations;
        // use bspcell
        split(root, cite);
        Debug.Log("t1 : " + (Time.realtimeSinceStartup - timer));
        // use bspcell
        offset(root);
        Debug.Log("t2 : " + (Time.realtimeSinceStartup - timer));
        // use bspcell
        toMap(root);
        Debug.Log("t3 : " + (Time.realtimeSinceStartup - timer));
        // use map
        connectCells(root.child1, root.child2);
        Debug.Log("t4 : " + (Time.realtimeSinceStartup - timer));
        // use map
        peelOuterLayer();
        Debug.Log("t5 : " + (Time.realtimeSinceStartup - timer));
        // use map
        entrance();
        Debug.Log("t6 : " + (Time.realtimeSinceStartup - timer));

        toTileMap();
        tileMeshIt();

        //if (meshit)
        // use map
            //meshIt();
        Debug.Log("t7 : " + (Time.realtimeSinceStartup - timer));
    }

    void toTileMap()
    {
        tileMap = new TileNode[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                tileMap[i, j] = new TileNode();

                tileMap[i, j].TileValue = map[i, j];
                tileMap[i, j].neighbor1 = hasNeighbor(i, j, 1);
            }

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                if (tileMap[i, j].neighbor1 && map[i, j] == 0)
                {
                    map[i, j] = 2;
                    tileMap[i, j].TileValue = 2;
                }
            }

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                tileMap[i, j].neighbor2 = hasNeighbor(i, j, 2);
    }

    void tileMeshIt()
    {
        var set = Resources.LoadAll<LocationTileSet>("TileSets/" + TileSet.ToString())[0];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                var tn = tileMap[i, j];
                if(tn.TileValue == 0)
                {
                    // outside walls
                    if (tn.neighbor2)
                    {
                        for (int y = 0; y < 5; y++)
                            CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.up * y, Quaternion.identity, parent.transform);
                    }
                }
                // inside with floor
                else if (tn.TileValue == 1)
                {
                    // ground
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.Ground], Vector3.right * i + Vector3.forward * j, Quaternion.identity, parent.transform);
                    // layer beneath ground
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.down, Quaternion.identity, parent.transform);
                    // roof
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.up * 5, Quaternion.identity, parent.transform);

                }
                // inside walls
                else if(tn.TileValue == 2)
                {
                    // layer beneath ground
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.down, Quaternion.identity, parent.transform);
                    // roof
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.up * 5, Quaternion.identity, parent.transform);
                    // inside wall
                    for (int y = 0; y < 5; y++)
                        CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.InnerWall], Vector3.right * i + Vector3.forward * j + Vector3.up * y, Quaternion.identity, parent.transform);
                }
            }

    }

    void meshIt()
    {
        var timer = Time.realtimeSinceStartup;
        Debug.Log("t7.0 : " + (Time.realtimeSinceStartup - timer));
        var set = Resources.LoadAll<LocationTileSet>("TileSets/" + TileSet.ToString())[0];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                if (map[i, j] == 1 )
                {

                    MonoBehaviour.Instantiate(set.GroundTiles[LocationTileSet.Ground], Vector3.right * i + Vector3.forward * j, Quaternion.identity, parent.transform);
                    
                }
                else if (hasNeighbor(i, j, 1) && i > 0)
                {
                    for (int y = 0; y < 4; y++)
                    {
                        map[i, j] = 2;
                        MonoBehaviour.Instantiate(set.GroundTiles[LocationTileSet.InnerWall], Vector3.right * i + Vector3.forward * j + Vector3.up * y, Quaternion.identity, parent.transform);
                        //t.localPosition = Vector3.right * i + Vector3.forward * j + Vector3.up * y;
                    }
                }

            }

        Debug.Log("t7.1 : " + (Time.realtimeSinceStartup - timer));
        outerLayer();
        Debug.Log("t7.2 : " + (Time.realtimeSinceStartup - timer));
    }

    void outerLayer ()
    {
        var set = Resources.LoadAll<LocationTileSet>("TileSets/" + TileSet.ToString())[0];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                if (map[i, j] == 0 && hasNeighbor(i, j, 2))
                {
                    //if (i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    for (int y = 0; y < 5; y++)
                    {
                        MonoBehaviour.Instantiate(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.up * y, Quaternion.identity, parent.transform);
                        //t.gameObject.layer = LayerMask.NameToLayer("Ship");
                    }

                }
                else if (map[i, j] > 0)
                {
                    MonoBehaviour.Instantiate(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.down, Quaternion.identity, parent.transform);
                    //t.gameObject.layer = LayerMask.NameToLayer("ShipTop");
                    MonoBehaviour.Instantiate(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.up * 5, Quaternion.identity, parent.transform);
                    //t.gameObject.layer = LayerMask.NameToLayer("ShipTop");
                }
            }
    }

    void entrance()
    {
        BspCell target = root;
        bool found = false;
        while (!found)
        {
            if (target.child1 != null)
                target = target.child1;
            else
                found = true;
        }
        for (int i = 0; i < target.x; i++)
            for (int j = target.y - halfCorridorSize; j <= target.y + halfCorridorSize; j++)
            {
                if (j > 0 && j < size)
                    map[i, j] = 1;
            }
    }

    void peelOuterLayer()
    {
        for (int i = 0; i < size; i++)
        {
            map[i, 0] = 0;
            map[i, 1] = 0;
            map[i, size - 1] = 0;
            map[i, size - 2] = 0;
        }
        for (int j = 0; j < size; j++)
        {
            map[0, j] = 0;
            map[1, j] = 0;
            map[size - 1, j] = 0;
            map[size - 2, j] = 0;
        }
    }

    void offset(BspCell cell)
    {
        if (cell.child1 != null && cell.child2 != null)
        {
            offset(cell.child1);
            offset(cell.child2);
        }
        else
        {
            var offsetw = Random.Range(1, cell.w / 2);
            var offseth = Random.Range(1, cell.h / 2);

            cell.w -= offsetw;
            cell.h -= offseth;
            cell.x += Random.Range(-offsetw, offsetw) / 2;
            cell.y += Random.Range(-offseth, offseth) / 2;
        }
    }

    void toMap(BspCell cell)
    {
        if (cell.child1 != null && cell.child2 != null)
        {
            toMap(cell.child1);
            toMap(cell.child2);
        }
        else
        {
            for (int i = cell.x - (cell.w) / 2; i < cell.x + (cell.w) / 2; i++)
                for (int j = cell.y - (cell.h) / 2; j < cell.y + (cell.h) / 2; j++)
                {
                    if (i > 0 && j > 0 && i < size && j < size)
                        map[i, j] = 1;
                }
        }
    }

    void split(BspCell cell, int i)
    {
        i--;
        if (i == 0)
            return;

        var dir = Random.value;
        if (dir > 0.5f)
        {
            if (cell.w / 2 > minRoomSize)
            {
                var nCell1 = new BspCell(cell.x - cell.w / 4, cell.y, cell.w / 2, cell.h);
                cell.child1 = nCell1;
                split(nCell1, i);

                var nCell2 = new BspCell(cell.x + cell.w / 4, cell.y, cell.w / 2, cell.h);
                cell.child2 = nCell2;
                split(nCell2, i);
            }
        }
        else
        {
            if (cell.h / 2 > minRoomSize)
            {
                var nCell1 = new BspCell(cell.x, cell.y - cell.h / 4, cell.w, cell.h / 2);
                cell.child1 = nCell1;
                split(nCell1, i);

                var nCell2 = new BspCell(cell.x, cell.y + cell.h / 4, cell.w, cell.h / 2);
                cell.child2 = nCell2;
                split(nCell2, i);
            }
        }
    }

    void connectCells(BspCell cell1, BspCell cell2)
    {
        if (cell2.x - cell1.x > cell2.y - cell1.y)
        {
            for (int i = cell1.x /*+ cell1.w/2*/; i <= cell2.x/* + cell1.w*/; i++)
                for (int j = cell1.y - halfCorridorSize; j <= cell1.y + halfCorridorSize; j++)
                {
                    if (i > 0 && j > 0 && i < size && j < size)
                        map[i, j] = 1;
                }
        }
        else
        {
            for (int i = cell1.y/* + cell1.h / 2*/; i <= cell2.y/* + cell1.h*/; i++)
                for (int j = cell1.x - halfCorridorSize; j <= cell1.x + halfCorridorSize; j++)
                {
                    if (i > 0 && j > 0 && i < size && j < size)
                        map[j, i] = 1;
                }
        }

        if (cell1.child1 != null && cell1.child2 != null)
            connectCells(cell1.child1, cell1.child2);
        if (cell2.child1 != null && cell2.child2 != null)
            connectCells(cell2.child1, cell2.child2);
        if (cell1.child1 != null && cell2.child1 != null)
            connectCells(cell1.child1, cell2.child1);
    }

    bool hasNeighbor(int i, int j, int val = 1)
    {
        for (int x = i - 1; x <= i + 1; x++)
            for (int y = j - 1; y <= j + 1; y++)
                if (x > 0 && y > 0 && x < size && y < size && map[x, y] == val)
                    return true;

        return false;
    }

    TileNode[,] tileMap;
    public struct TileNode
    {
        public int TileValue;
        public bool neighbor1;
        public bool neighbor2;
    }
}

public class BspCell
{
    public int x;
    public int y;

    public int w;
    public int h;

    public BspCell child1;
    public BspCell child2;
    public BspCell(int _x, int _y, int _w, int _h)
    {
        x = _x;
        y = _y;
        w = _w;
        h = _h;
    }
}
