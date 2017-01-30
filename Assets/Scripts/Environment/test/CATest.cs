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
    public List<Cell> Rooms = new List<Cell>();
    public float MinSize = 5;
    int[,,] map;
    Mesh output;
    MeshDraft draft;
    // Use this for initialization
    void Start()
    {
        output = new Mesh();
    }

    void OnGUI()
    {
        if(GUILayout.Button("DO CA"))
        { 
            Generate();
            doRooms();
        }

        if (GUILayout.Button("DO MESH"))
            CreateAsMesh();
        
        ShowGizmos = GUILayout.Toggle(ShowGizmos, "Show gizmos");
    }
        
    void CreateAsMesh()
    {
        draft = new MeshDraft();
        
        int counter = 0;
        foreach (var r in Cells)
        { 
            var e = MeshDraft.Hexahedron(r.w, r.h, 1);
            e.Move(Vector3.forward * r.center.y + Vector3.right * r.center.x);// + counter++ *Vector3.up);
            draft.Add(e);
        }

        // connect
        

        output = draft.ToMesh();
        GetComponent<MeshFilter>().mesh = output;
    }

    void ConnectRooms(Cell c)
    {
        if(c.hasChildren && c.Child1.hasChildren)
        {
            ConnectRooms(c.Child1);
            ConnectRooms(c.Child2);
        }
        else
        {
            //var distx = Mathf.Abs(c.Child1.x - c.Child2.x) - c.Child1.w - c.Child2.w;
            //var e= MeshDraft.Hexahedron(distx, 1, 1);
            
        }
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
        if (i <= 0) return;
        Cell cell1;
        Cell cell2;
        var r = Random.Range(0, 100);
        if (r < 50)
        {
            var t = cell.w / Random.Range(1f,3f);
            if (t > MinSize && cell.w - t > MinSize)
            {
                cell1 = new Cell(cell.x, cell.y, t, cell.h);
                cell2 = new Cell(cell.x + t, cell.y,cell.w- t, cell.h);
                cell.hasChildren = true;
                Cells.Add(cell1);
                Cells.Add(cell2);
                cell.Child1 = cell1;
                cell.Child2 = cell2;

                splitCell(cell1, i);
                splitCell(cell2, i);
            }
        }
        else 
        {
            var t = cell.h / Random.Range(1f, 3f);
            if (t > MinSize && cell.h - t > MinSize)
            {
                cell1 = new Cell(cell.x, cell.y, cell.w, t);
                cell2 = new Cell(cell.x, cell.y + t, cell.w, cell.h - t);
                cell.hasChildren = true;
                Cells.Add(cell1);
                Cells.Add(cell2);
                cell.Child1 = cell1;
                cell.Child2 = cell2;

                splitCell(cell1, i);
                splitCell(cell2, i);
            }
        }
    }

    void doRooms()
    {
        var toRooms = Cells.Where(m => !m.hasChildren);
        foreach(var c in toRooms)
        {
            var r = new Cell(c.x, c.y, c.w, c.h);

            var widthOff = Random.Range(1f, c.w / 2f);
            var heightOff = Random.Range(1f, c.h / 2f);
            r.w -= widthOff;
            r.x += Random.value * widthOff;
            r.h -= heightOff;
            r.h += Random.value * heightOff;

            //Rooms.Add(r);
        }
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

        public bool hasChildren = false;
        public Cell Child1;
        public Cell Child2;

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
