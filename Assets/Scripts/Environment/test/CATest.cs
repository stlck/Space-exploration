using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralToolkit;using System.Linq;

public class CATest : MonoBehaviour
{
    public int Size = 30;
    public int CutIterations = 4;
    public bool ShowGizmos = false;

    public List<Cell> Cells = new List<Cell>();
    public List<Room> Rooms = new List<Room>();
    int[,,] map;
    Mesh output;

    // Use this for initialization
    void Start()
    {
        output = new Mesh();
    }

    void OnGUI()
    {
        if(GUILayout.Button("DO CA"))
            Generate();

        if (GUILayout.Button("DO ROOMS"))
            doRooms();

        if (GUILayout.Button("DO MESH"))
            CreateAsMesh();
        
        ShowGizmos = GUILayout.Toggle(ShowGizmos, "Show gizmos");
    }
        
    void CreateAsMesh()
    {
        MeshDraft draft = new MeshDraft();

        //for (int i = 0; i < Size; i++)
        //    for (int j = 0; j < Size; j++)
        //        for (int k = 0; k < Size; k++)
        //        {
        //            if(map[i,j,k] > 0)
        //            {
        //                var e = MeshDraft.Hexahedron(1, 1, 1);
        //                e.Move(indexToPosition(i,j,k));
        //                draft.Add(e);
        //            }
        //        }
        int counter = 0;
        foreach (var r in Rooms)
        { 
            var e = MeshDraft.Hexahedron(r.w, r.h, 1);
            e.Move(Vector3.forward * r.center.y + Vector3.right * r.center.x + counter++ * Vector3.up);
            draft.Add(e);
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
        Rooms.Clear();
        Cells.Clear();
        int iterations = CutIterations;
        var start = new Cell( 0, 0, Size, Size);
        Cells.Add(start);
        splitCell(start, iterations);
    }
    
    void splitCell(Cell cell, int i)
    {
        i--;
        Cell cell2;
        if(Random.value > 0.5f)
        {
            var t = cell.w / 2;// Random.Range(2,4);
            cell2 = new Cell(cell.x + t, cell.y, cell.w - t, cell.h);
            cell.w -= t;
        }
        else
        {
            var t = cell.h / 2;// Random.Range(2, 4);
            cell2 = new Cell(cell.x, cell.y + t, cell.w, cell.h - t);
            cell.h -= t;
        }
        Cells.Add(cell2);

        if(i > 0)
        {
            splitCell(cell, i);
            splitCell(cell2, i);
        }
    }

    void doRooms()
    {
        foreach(var c in Cells)
        {
            var r = new Room(c.x, c.y, c.w, c.h);
            Rooms.Add(r);
        }
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
        
        if (Rooms != null && Rooms.Count > 0 && ShowGizmos)
            foreach (var c in Rooms)
            {
                Gizmos.color = new Color(c.x / (float)Size, c.y /(float)Size,1f,1f);
                Gizmos.DrawCube(Vector3.forward * c.center.y + Vector3.right * c.center.x, Vector3.forward * c.h + Vector3.right * c.w + Vector3.up);
            }
        else if(Cells != null && ShowGizmos)
            foreach (var c in Cells)
            {
                Gizmos.color = new Color(c.x / (float)Size, c.y / (float)Size, 1f, 1f);
                Gizmos.DrawCube(Vector3.forward * c.center.y + Vector3.right * c.center.x, Vector3.forward * c.h + Vector3.right * c.w + Vector3.up);
                
            }
    }

    [System.Serializable]
    public class Room
    {
        public float x;
        public float y;
        public float w;
        public float h;

        public Room(float _x, float _y, float _w, float _h)
        {
            this.x = _x;
            this.y = _y;
            this.w = _w;
            this.h = _h;
        }

        public Point center
        {
            get
            {
                return new Point(x + w / 2, y + h / 2);
            }
        }
    }

    [System.Serializable]
    public class Cell
    {
        public float x;
        public float y;
        public float w;
        public float h;

        public Point center
        {
            get
            {
                return new Point(x + w / 2, y + h / 2);
            }
        }

        public Cell(float _x, float _y, float _w, float _h)
        {
            this.x = _x;
            this.y = _y;
            this.w = _w;
            this.h = _h;
        }
    }

    [System.Serializable]
    public class Point
    {
        public float x;
        public float y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
