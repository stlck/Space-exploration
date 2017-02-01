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
        GUILayout.TextArea(@"For ships, calc all positions. for each square (x,y,z)*4, add planes where == 1\n
     \n     o----o
     \n    /    /|
     \n   o----o |
     \n   |    | o
     \n   |    |/
     \n   o----o
");
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
        foreach (var r in Cells.Where(m => !m.hasChildren))
        { 
            var e = MeshDraft.Hexahedron(r.w, r.h, 1);
            e.Move(Vector3.forward * r.center.y + Vector3.right * r.center.x);// + counter++ * Vector3.up);
            draft.Add(e);
        }

        // connect
        ConnectRooms(Cells[0],draft);

        output = draft.ToMesh();
        GetComponent<MeshFilter>().mesh = output;
    }
    float corridorMinSize = 5f;

    void ConnectRooms(Cell c, MeshDraft draft)
    {
        if (c.hasChildren && c.Child1.hasChildren)
        {
            ConnectRooms(c.Child1, draft);
            ConnectRooms(c.Child2, draft);
            if(c.Child1.Child1 != null && c.Child2.Child1 != null)
            {
                //Debug.Log(c.Child1.Child1 + " \nto\n" + c.Child2.Child1);
                connectCells2(c.Child1.Child1, c.Child2.Child1, draft);
            }
            else if( c.Child1.Child2 != null && c.Child2.Child2 != null)
            {
                //Debug.Log(c.Child1.Child2 + " \nto\n" + c.Child2.Child2);
                connectCells2(c.Child1.Child2, c.Child2.Child2, draft);
            }
        }
        if (c.hasChildren && !c.Child1.hasChildren)
        {
            connectCells(c.Child1, c.Child2, draft);
        }
    }

    void connectCells2(Cell cell1, Cell cell2, MeshDraft draft)
    {
        Bounds b1 = new Bounds(cell1.center.Position, cell1.Size);
        Bounds b2 = new Bounds(cell2.center.Position, cell2.Size);
        if(cell1.hasChildren && cell2.hasChildren)
        {
            if(cell1.Child2 != null && cell2.Child1 != null)
                connectCells2(cell1.Child2, cell2.Child1, draft);
            else if(cell1.Child1 != null && cell2.Child2 != null)
                connectCells2(cell1.Child1, cell2.Child2, draft);
        }
        else if (Mathf.Clamp(cell1.x, cell2.x, cell2.x + cell2.w) == cell1.x)
        {
            //var height = Mathf.Abs(cell1.y - cell2.y) - cell1.h / 2f - cell2.h / 2f;
            var width = corridorMinSize;
            var height = cell2.y - (cell1.y + cell1.h);//cell1.x > cell2.x ? cell1.x : cell2.x;

            var x1 = cell2.x;
            var x2 = cell1.x + cell1.w;
            var x = Mathf.Lerp(x1, x2, .5f);

            var y1 = cell1.y + cell1.h;//cell2.y > cell1.y ? cell1.y: cell2.y;
            var y2 = cell2.y;// + cell2.h > cell1.y + cell1.h ? cell1.y + cell1.h: cell2.y + cell2.h;
            var y = Mathf.Lerp(y1, y2, .5f);
            
            //var prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //prim.transform.position = Vector3.forward * (y - height/2f) + Vector3.right * (x - width/2f);
            //prim.name = "x1y1";
            //prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //prim.transform.position = Vector3.forward * (y - height / 2f) + Vector3.right * (x + width / 2f);
            //prim.name = "x2y1";
            //prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //prim.transform.position = Vector3.forward * (y + height / 2f) + Vector3.right * (x - width / 2f);
            //prim.name = "x1y2";
            //prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //prim.transform.position = Vector3.forward * (y + height / 2f) + Vector3.right * (x + width / 2f);
            //prim.name = "x2y2";

            var e = MeshDraft.Hexahedron(width, height, 1);
            e.Move(Vector3.right * (x)  + Vector3.forward * (y) + Vector3.up);
            draft.Add(e);

            //Debug.Log("Cell1 left side in cell2. size : " + corridorMinSize + "x" + (y2 - y1) + " start at " + (cell2.x + cell2.w / 2f) + "x" + (cell1.y + cell1.h / 2f));
            Debug.Log("Cell1 left side in cell2 \ncenter: " + x + "x" + y + "\nwh: " + width + "x" + height + "\n cell1: " + cell1 + "\ncell2:" + cell2);

        }
        else if (Mathf.Clamp(cell1.x + cell1.w, cell2.x, cell2.x + cell2.w) == cell1.x + cell1.w)
        {
            //Debug.Log("Cell1 right side in cell2");
            var width = corridorMinSize;
            var height = cell2.y - (cell1.y + cell1.h);

            var x1 = cell1.x;
            var x2 = cell2.x + cell2.w;
            var x = Mathf.Lerp(x1, x2, .5f);

            var y1 = cell1.y + cell1.h;//cell2.y > cell1.y ? cell1.y: cell2.y;
            var y2 = cell2.y;// + cell2.h > cell1.y + cell1.h ? cell1.y + cell1.h: cell2.y + cell2.h;
            var y = Mathf.Lerp(y1, y2, .5f);

            var e = MeshDraft.Hexahedron(width, height, 1);
            e.Move(Vector3.right * (x) + Vector3.forward * (y) + Vector3.up);
            draft.Add(e);
        }
        else if (Mathf.Clamp(cell1.y, cell2.y, cell2.y + cell2.h) == cell1.y)
        {
            //Debug.Log("Cell1 lower side in cell2");
            var width = cell2.x - (cell1.x + cell1.w); 
            var height = corridorMinSize;

            var y1 = cell2.y;
            var y2 = cell1.y + cell1.h;
            var y = Mathf.Lerp(y1, y2, .5f);

            var x1 = cell1.x + cell1.w;//cell2.y > cell1.y ? cell1.y: cell2.y;
            var x2 = cell2.x;// + cell2.h > cell1.y + cell1.h ? cell1.y + cell1.h: cell2.y + cell2.h;
            var x = Mathf.Lerp(x1, x2, .5f);

            var e = MeshDraft.Hexahedron(width, height, 1);
            e.Move(Vector3.right * (x) + Vector3.forward * (y) + Vector3.up);
            draft.Add(e);
        }
        else if (Mathf.Clamp(cell1.y + cell1.h, cell2.y, cell2.y + cell2.h) == cell1.y + cell1.h)
        {
            //Debug.Log("Cell1 upper side in cell2");
            var width = cell2.y - (cell1.y + cell1.h);
            var height = corridorMinSize;

            var y1 = cell1.y;
            var y2 = cell2.y + cell2.h;
            var y = Mathf.Lerp(y1, y2, .5f);

            var x1 = cell1.x + cell1.w;//cell2.y > cell1.y ? cell1.y: cell2.y;
            var x2 = cell2.x;// + cell2.h > cell1.y + cell1.h ? cell1.y + cell1.h: cell2.y + cell2.h;
            var x = Mathf.Lerp(x1, x2, .5f);

            var e = MeshDraft.Hexahedron(width, height, 1);
            e.Move(Vector3.right * (x) + Vector3.forward * (y) + Vector3.up);
            draft.Add(e);
        }
    }

    void connectCells(Cell cell1, Cell cell2, MeshDraft draft)
    {
        Bounds b1 = new Bounds(cell1.center.Position, cell1.Size);
        Bounds b2 = new Bounds(cell2.center.Position, cell2.Size);

        var b1Point = b1.ClosestPoint(cell2.center.Position);
        var b2Point = b2.ClosestPoint(cell1.center.Position);

        var xSize = Mathf.Abs(b1Point.x - b2Point.x);
        if (Mathf.Abs(cell1.x - cell2.x) < 1)
            xSize = corridorMinSize;
        var zSize = Mathf.Abs(b1Point.z - b2Point.z);
        if (Mathf.Abs(cell1.y - cell2.y) < 1)
            zSize = corridorMinSize;

        var moveToCenter = Vector3.Lerp(b1Point, b2Point, .5f);
        //Debug.Log("connection : " + b1Point + " to " + b2Point + "\n w " + xSize + " h " + zSize + "\ncenter: " + moveToCenter);

        var e = MeshDraft.Hexahedron(xSize, zSize, 1f);
        e.Move(moveToCenter + Vector3.up);
        draft.Add(e);
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
            var t = cell.w / 2f;// * Random.Range(.3f,.6f);
            if (t > MinSize && cell.w - t > MinSize)
            {
                cell1 = new Cell(cell.x, cell.y, t, cell.h);

                var nX = cell.x + t;
                var nW = cell.w - t;
                cell2 = new Cell(nX, cell.y,nW, cell.h);

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
            var t = cell.h / 2f;// * Random.Range(.3f, .6f);
            if (t > MinSize && cell.h - t > MinSize)
            {
                cell1 = new Cell(cell.x, cell.y, cell.w, t);

                var nY = cell.y + t;
                var nH = cell.h - t;
                cell2 = new Cell(cell.x, nY, cell.w, nH);

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

            var widthOff = Random.Range(1, ((int)c.w-1) / 2);
            var heightOff = Random.Range(1, ((int)c.h-1) / 2);
            c.w -= widthOff;
            //c.x += Random.value * widthOff;
            c.h -= heightOff;
            //c.h += Random.value * heightOff;

            var r = new Cell(c.x, c.y, c.w, c.h);
            Rooms.Add(r);
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
    public class Cell
    {
        public float x;
        public float y;
        public Vector3 Position
        {
            get
            {
                return Vector3.forward * y + Vector3.right * x;
            }
        }
        public float w;
        public float h;
        public Vector3 Size
        {
            get
            {
                return Vector3.forward * h + Vector3.right * w;
            }
        }

        public bool hasChildren = false;
        public Cell Child1;
        public Cell Child2;

        //public Point CornerLowerLeft
        //{
        //    get
        //    {
        //        return new Point(x, y);
        //    }
        //}

        //public Point CornerUpperLeft
        //{
        //    get
        //    {
        //        return new Point(x, y + h);
        //    }
        //}


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

        public override string ToString()
        {
            return "x:" + x + " . y:" + y + " . w: " + w + " . h:" + h + "\nCenter: " + center;
        }
    }

    [System.Serializable]
    public class Point
    {
        public float x;
        public float y;
        public Vector3 Position
        {
            get
            {
                return Vector3.forward * y + Vector3.right * x;
            }
        }

        public Point(float _x, float _y)
        {
            this.x = _x;
            this.y = _y;
        }

        public static Point operator -(Point a, Point b)
        {
            var ret = new Point(a.x - b.x, a.y - b.y);

            return ret;
        }

        public static Point operator +(Point a, Point b)
        {
            var ret = new Point(a.x + b.x, a.y + b.y);

            return ret;
        }
        public override string ToString()
        {
            return "x:" + x + " . y:" + y;
        }
    }
}
