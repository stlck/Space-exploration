using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public List<BspCell> Rooms;

    public int[,] Generate(Transform _parent, int seed, int _size, int _splits = 5, int _minRoomSize = 8, int _halfCorridorSize = 1, TileSet _tileSet = TileSet.BlackAsteroid, bool buildmesh = true)
    {
        size = _size;
        map = new int[size, size];

        iterations = _splits;
        minRoomSize = _minRoomSize;
        halfCorridorSize = _halfCorridorSize;
        parent = _parent;
        TileSet = _tileSet;

        doAll(buildmesh);
        
        return map;
    }

    public int[,,] ToVoxelMap(int height = 10)
    {
        int[,,] voxelMap = new int[size+2, height, size+2];
        for (int x = 1; x < size+1; x++)
            for (int z = 1; z < size+1; z++)
                for (int y = 1; y < height; y++)
                {
                    var tn = tileMap[x - 1, z - 1];
                    if (tn.TileValue == 0 && tn.neighbor2 && y <= 5)
                    {
                        // outside walls
                            voxelMap[x, y, z] = 3;
                    }
                    // inside with floor
                    else if (tn.TileValue == 1 && y == 2)
                    {
                        voxelMap[x, y, z] = 1;
                        // ground
                        // layer beneath ground
                        // roof
                    }
                    // inside walls
                    else if (tn.TileValue == 2 && y >= 2 && y <= 5)
                    {
                        // layer beneath ground
                        // roof
                        // inside wall
                        //for (int y = 0; y < 5; y++)
                        voxelMap[x, y, z] = 2;
                    }
                    else
                        voxelMap[x, y, z] = 0;
                }

        return voxelMap;
    }

    public int[,] GenerateIndirect(int _size, int _splits = 5, int _minRoomSize = 8, int _halfCorridorSize = 1)
    {
        size = _size;
        map = new int[size, size];

        iterations = _splits;
        minRoomSize = _minRoomSize;
        halfCorridorSize = _halfCorridorSize;

        root = new BspCell(size / 2, size / 2, size, size);
        Rooms = new List<BspCell>();
        var cite = iterations;

        split(root, cite);
        offset(root);
        toMap(root);
        connectCells(root.child1, root.child2);
        peelOuterLayer();
        entrance();
        toTileMap();

        return map;
    }

    void doAll(bool meshit)
    {
        var timer = Time.realtimeSinceStartup;
        //Debug.Log("t1 : " + (Time.realtimeSinceStartup - timer));
        root = new BspCell(size / 2, size / 2, size, size);
        Rooms = new List<BspCell>();
        var cite = iterations;
        // use bspcell
        split(root, cite);
        //Debug.Log("t1 : " + (Time.realtimeSinceStartup - timer));
        // use bspcell
        offset(root);
        //Debug.Log("t2 : " + (Time.realtimeSinceStartup - timer));
        // use bspcell
        toMap(root);
        //Debug.Log("t3 : " + (Time.realtimeSinceStartup - timer));
        // use map
        connectCells(root.child1, root.child2);
        //Debug.Log("t4 : " + (Time.realtimeSinceStartup - timer));
        // use map
        peelOuterLayer();
        //Debug.Log("t5 : " + (Time.realtimeSinceStartup - timer));
        // use map
        entrance();
        //Debug.Log("t6 : " + (Time.realtimeSinceStartup - timer));

        toTileMap();
        if(meshit)
            tileMeshIt();

        Testing.AddDebug("LocGen time: " + (Time.realtimeSinceStartup - timer));
    }

    void toTileMap()
    {
        tileMap = new TileNode[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                tileMap[i, j] = new TileNode();
                tileMap[i, j].x = i;
                tileMap[i, j].y = j;
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

        foreach(var r in Rooms)
        {
            r.TileNodes = new List<TileNode>();
            foreach (var tile in r.CellTiles)
                r.TileNodes.Add(tileMap[tile.x, tile.y]);
        }
    }

    void tileMeshIt()
    {
        var time = Time.realtimeSinceStartup;
        Debug.Log("TIme start");
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
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.InnerWall], Vector3.right * i + Vector3.forward * j + Vector3.up * 5, Quaternion.identity, parent.transform, 9);
                    
                }
                // inside walls
                else if(tn.TileValue == 2)
                {
                    // layer beneath ground
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.down, Quaternion.identity, parent.transform);
                    // roof
                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.InnerWall], Vector3.right * i + Vector3.forward * j + Vector3.up * 5, Quaternion.identity, parent.transform, 9);
                    // inside wall
                    for (int y = 0; y < 5; y++)
                        CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.Room], Vector3.right * i + Vector3.forward * j + Vector3.up * y, Quaternion.identity, parent.transform);
                }
            }

        Debug.Log("TIme stop at " + (Time.realtimeSinceStartup - time));
    }

    #region testtilebuild
    void manualTileBuild()
    {
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                
                //var tileMap[i, j] = tileMap[i, j];
                tileMap[i, j].neighbors = 0;
                tileMap[i, j].neighborcount = 0;
                //if (tileMap[i, j].TileValue == 2)
                {
                    if (i > 0 && tileMap[i - 1, j].TileValue == 2)
                    {
                        tileMap[i, j].neighbors |= NeighborEnum.left;
                        tileMap[i, j].neighborcount++;
                    }

                    if (j > 0 && tileMap[i, j - 1].TileValue == 2)
                    {
                        tileMap[i, j].neighbors |= NeighborEnum.back;
                        tileMap[i, j].neighborcount++;
                    }

                    if (i < size - 1 && tileMap[i + 1, j].TileValue == 2)
                    {
                        tileMap[i, j].neighbors |= NeighborEnum.right;
                        tileMap[i, j].neighborcount++;
                    }

                    if (j < size - 1 && tileMap[i, j + 1].TileValue == 2)
                    {
                        tileMap[i, j].neighbors |= NeighborEnum.forward;
                        tileMap[i, j].neighborcount++;
                    }

                }
            }
        
        ProceduralToolkit.MeshDraft draft = new ProceduralToolkit.MeshDraft(parent.GetComponent<MeshFilter>().mesh);
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                var tn = tileMap[i, j];
                switch(tn.TileValue)
                {
                    case 0:
                        //draft.Add(ProceduralToolkit.MeshDraft.Quad(Vector3.right * i + Vector3.forward * j + Vector3.down, Vector3.one, Vector3.one));
                       
                        break;
                    case 1:
                        if (tn.neighborcount == 2)
                        {
                            var p = Vector3.right * i + Vector3.forward * j;// + Vector3.right/2 + Vector3.forward/2;
                            if ((tn.neighbors & NeighborEnum.left) == NeighborEnum.left && (tn.neighbors & NeighborEnum.back) == NeighborEnum.back)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p + Vector3.right, p, p + Vector3.forward));
                            }
                            else if ((tn.neighbors & NeighborEnum.left) == NeighborEnum.left && (tn.neighbors & NeighborEnum.forward) == NeighborEnum.forward)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p, p + Vector3.forward, p + Vector3.forward + Vector3.right));
                            }
                            else if ((tn.neighbors & NeighborEnum.right) == NeighborEnum.right && (tn.neighbors & NeighborEnum.back) == NeighborEnum.back)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p + Vector3.right, p, p + Vector3.forward + Vector3.right));
                            }
                            else if ((tn.neighbors & NeighborEnum.right) == NeighborEnum.right && (tn.neighbors & NeighborEnum.forward) == NeighborEnum.forward)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p + Vector3.right + Vector3.forward, p + Vector3.right, p + Vector3.forward));
                            }
                            else
                                draft.Add(ProceduralToolkit.MeshDraft.Quad(Vector3.right * i + Vector3.forward * j, Vector3.right, Vector3.forward));
                        }
                        //draft.Add(ProceduralToolkit.MeshDraft.Quad(Vector3.right * i + Vector3.forward * j, Vector3.one, Vector3.one));
                        break;
                    case 2:
                        if(tn.neighborcount == 2)
                        {
                            var p = Vector3.right * i + Vector3.forward * j;// + Vector3.right/2 + Vector3.forward/2;
                            if ((tn.neighbors & NeighborEnum.left) == NeighborEnum.left && (tn.neighbors & NeighborEnum.back) == NeighborEnum.back)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p + Vector3.right, p, p + Vector3.forward));
                            }
                            else if ((tn.neighbors & NeighborEnum.left) == NeighborEnum.left && (tn.neighbors & NeighborEnum.forward) == NeighborEnum.forward)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p, p + Vector3.forward, p + Vector3.forward + Vector3.right));
                            }
                            else if ((tn.neighbors & NeighborEnum.right) == NeighborEnum.right && (tn.neighbors & NeighborEnum.back) == NeighborEnum.back)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p + Vector3.right, p, p + Vector3.forward + Vector3.right));
                            }
                            else if ((tn.neighbors & NeighborEnum.right) == NeighborEnum.right && (tn.neighbors & NeighborEnum.forward) == NeighborEnum.forward)
                            {
                                draft.Add(ProceduralToolkit.MeshDraft.Triangle(p + Vector3.right + Vector3.forward, p + Vector3.right, p + Vector3.forward));
                            }
                            else
                                draft.Add(ProceduralToolkit.MeshDraft.Quad(Vector3.right * i + Vector3.forward * j, Vector3.right, Vector3.forward));
                        }
                        else
                            draft.Add(ProceduralToolkit.MeshDraft.Quad(Vector3.right * i + Vector3.forward * j, Vector3.right, Vector3.forward));

                        break;
                    default:
                        break;
                }
            }
        parent.GetComponent<MeshFilter>().mesh = draft.ToMesh();
    }
    #endregion

    #region old method
    void meshIt ()
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
    #endregion

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
            int cellStartX = cell.x - (cell.w) / 2;
            int cellEndX = cell.x + (cell.w) / 2;
            int cellStartY = cell.y - (cell.h) / 2;
            int cellEndY = cell.y + (cell.h) / 2;
            var cellCount = 0;
            cell.CellTiles = new List<Vector2Int>();
            //List<Vector2Int> RoomTiles = new List<Vector2Int>();

            for (int i = cellStartX; i < cellEndX; i++)
                for (int j = cellStartY; j < cellEndY; j++)
                {
                    if (i > 0 && j > 0 && i < size && j < size)
                    {
                        map[i, j] = 1;
                        cellCount++;
                        cell.CellTiles.Add(new Vector2Int(i, j));
                    }
                }
            
            Rooms.Add(cell);

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
        //for (int x = i - 1; x <= i + 1; x++)
        //    for (int y = j - 1; y <= j + 1; y++)
        //        if (x > 0 && y > 0 && x < size && y < size && map[x, y] == val)
        //            return true;

        if (i > 0 && map[i - 1, j] == val)
            return true;
        else if (i < size - 1 && map[i + 1, j] == val)
            return true;
        else if (j > 0 && map[i, j - 1] == val)
            return true;
        else if (j < size - 1 && map[i, j + 1] == val)
            return true;

        return false;
    }

    public TileNode[,] tileMap;
   
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

    public List<Vector2Int> CellTiles;
    public List<TileNode> TileNodes;
}
public struct TileNode
{
    public int x;
    public int y;
    /// <summary>
    /// 0 = nothing
    /// 1 = floor
    /// 2 = walls
    /// 3 = floor with extra stuff on
    /// </summary>
    public int TileValue;
    public bool neighbor1;
    public bool neighbor2;
    public NeighborEnum neighbors;
    public int neighborcount;
}

[System.Flags]
public enum NeighborEnum
{
    forward = 1,
    right = 2,
    left = 4,
    back = 8
}