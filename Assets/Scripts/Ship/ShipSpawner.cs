using ProceduralToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShipSpawner : MonoBehaviour {

    public Vector2Int Size = new Vector2Int(11, 14);
    public MeshFilter Target;
    public static List<GameObject> ControlList = new List<GameObject>();
    public DockingPoint TargetDock;

    int[,] tiles;
    int[,] controls;
    bool changed;

    bool doGround = true;
    public bool IsTesting = false;
    public bool Show = false;

    int currentControl;
    Vector3 center;
    Vector2 scrollPosition;

    // Use this for initialization
    public void Start()
    {
        reset();

        ControlList = Resources.LoadAll<GameObject>("ShipControls").ToList();

        if (ControlList.Any())
            currentControl = 0;
    }

    void OnMouseUp()
    {
        if (!canShow())
            RangeIndicator.Instance.TurnOn(transform.position, 3);
        else
        {
            if (!Show)
                Show = true;
            else
                Show = false;
        }
    }

    bool canShow()
    {
        return Vector3.Distance(MyAvatar.Instance.transform.position, transform.position) < 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsTesting && Show && !canShow() || Input.GetKeyDown(KeyCode.Escape))
            Show = false;
    }

    public static void SrvCreateShip(int[,] t, int[,] c, int sizex, int sizey, Vector3 position, Quaternion rotation)
    {
        var baseShip = Resources.Load<Ship>("BaseShip");
        var s = Instantiate(baseShip, Vector3.zero, Quaternion.identity);//Target.gameObject.AddComponent<Ship>();
        NetworkServer.Spawn(s.gameObject);
        var center = new Vector3(sizex / 2, 0, sizey / 2);

        for (int y = 1; y < sizey - 1; y++)
            for (int x = 1; x < sizex - 1; x++)
                if (c[x, y] >= 0)
                {
                    var spawn = new NetworkSpawnObject();
                    spawn.SpawnTarget = ControlList[c[x, y]];
                    spawn.Position = positionOf(x, y) + Vector3.left * center.x + Vector3.forward * center.z;
                    spawn.PositionIsLocal = true;
                    spawn.Parent = s.gameObject;
                    s.NetworkSpawnObjects.Add(spawn);
                }

        s.transform.position = position;
        s.transform.rotation = rotation;

        s.tiles = t;
        s.Sizex = sizex;
        s.Sizey = sizey;
    }

    public void OnGUI()
    {
        if (!Show) return;

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        changed = false;

        if (GUILayout.Button("RESET"))
            reset();

        if (GUILayout.Button("Cmd! Create Ship"))
            MyAvatar.Instance.CmdSpawnShip(tiles, controls, Size.x, Size.y, TargetDock.DockAlign.position, TargetDock.DockAlign.rotation);//createShip();

        if (GUILayout.Button("test create ship2"))
            createShipTest();

        doGround = GUILayout.Toggle(doGround,doGround ? "HULL" : "Controls");
        if(doGround)
        { 
            for (int y = 1; y < Size.y - 1; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 1; x < Size.x-1; x++)
                {
                    if (y == center.z && x == center.x)
                        GUI.contentColor = Color.green;
                    else if(tiles[x,y] > 0)
                        GUI.contentColor = Color.white;
                    else
                        GUI.contentColor = Color.grey;

                    if (GUILayout.Button(tiles[x, y].ToString(), GUILayout.Width(25)) && hasNeighbor(x,y) > 0)
                    {
                        tiles[x, y] = tiles[x, y] == 1 ? 0 : 1;
                        if (tiles[x, y] == 0)
                            controls[x, y] = -1;
                        changed = true;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        else {
            int index = 0;
            foreach (var c in ControlList)
            { 
                if (GUILayout.Button(index + ":\t" + c.name))
                { 
                    currentControl = index;
                }
                index++;
            }
            for (int y = 1; y < Size.y - 1; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 1; x < Size.x-1; x++)
                {
                    if (tiles[x, y] == 1)
                    {
                        if(hasNeighbor(x,y) > 2)
                        {
                            if(controls[x, y] == -1)
                                GUI.contentColor = Color.green;
                            else
                                GUI.contentColor = Color.white;

                            if (controls[x, y] == -1 && GUILayout.Button("x", GUILayout.Width(25)) && hasNeighbor(x, y) > 2)
                            {
                                controls[x, y] = currentControl;//ControlList.IndexOf(currentControl);
                            }
                            else if (controls[x, y] >= 0 && GUILayout.Button(controls[x, y].ToString(), GUILayout.Width(25)))
                                controls[x, y] = -1;
                        }
                        else
                        {
                            GUI.contentColor = Color.red;
                            GUILayout.Button(controls[x, y].ToString(), GUILayout.Width(25));
                        }
                    }
                    else
                        GUILayout.Space(30);
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        if (IsTesting && changed)
        {
            ShipToMesh(Target, Size.x, Size.y, tiles);
        }
    }

    void createShipTest()
    {
        ShipToMesh(Target, Size.x, Size.y, tiles);
    }

    int hasNeighbor(int x, int y)
    {
        int ret = 0;
        if (x - 1 >= 0 && tiles[x - 1, y] == 1)
            ret++;
        if (x + 1 < Size.x && tiles[x + 1, y] == 1)
            ret++;
        if (y - 1 >= 0 && tiles[x, y - 1] == 1)
            ret++;
        if (y + 1 < Size.y && tiles[x, y + 1] == 1)
            ret++;
        return ret;
    }

    void reset()
    {
        tiles = new int[Size.x, Size.y];
        controls = new int[Size.x, Size.y];
        center = new Vector3((int)Size.x / 2, 0, (int)Size.y / 2);
        
        changed = true;
        for (int x = 0; x < Size.x; x++)
            for(int y = 0; y < Size.y; y++)
            {
                tiles[x,y] = 0;
                controls[x, y] = -1;
            }

        tiles[(int)center.x, (int)center.z] = 1;
    }

    public static void ShipToMesh(MeshFilter _target, int _sizex, int _sizey, int[,] _tiles)
    {
        var center = new Vector3((int)_sizex / 2, 0, (int)_sizey / 2);
        MeshDraft draft = new MeshDraft();
        Mesh m = new Mesh();
        List<Vector3> nodeTiles = new List<Vector3>();
        for (int x = 0; x < _sizex-1; x++)
            for (int y = 0; y < _sizey-1; y++)
            {
                if (_tiles[x, y ] > 0)
                    nodeTiles.Add(positionOf(x, y));
                if (_tiles[x, y+1] > 0)
                    nodeTiles.Add(positionOf(x,  y + 1));
                if (_tiles[x+1, y + 1] > 0)
                    nodeTiles.Add(positionOf(x+1, y + 1));
                if (_tiles[x+1, y] > 0)
                    nodeTiles.Add(positionOf(x+1, y));

                if (nodeTiles.Count == 4)
                {
                    draft.Add(MeshDraft.Quad(nodeTiles[3], nodeTiles[2], nodeTiles[1], nodeTiles[0]));
                    //draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[2], nodeTiles[3]));
                    
                }
                else if (nodeTiles.Count == 3)
                {
                    draft.Add(MeshDraft.Triangle(nodeTiles[2], nodeTiles[1], nodeTiles[0]));
                    //Debug.Log(nodeTiles[0] + " x " + nodeTiles[1] + " x " + nodeTiles[2]);
                    if(nodeTiles[0].x != nodeTiles[1].x && nodeTiles[0].z != nodeTiles[1].z)
                    {
                        draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up, nodeTiles[1] + Vector3.up, nodeTiles[1]));
                        draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] + Vector3.up, nodeTiles[0] + Vector3.up));
                    }
                    else if (nodeTiles[0].x != nodeTiles[2].x && nodeTiles[0].z != nodeTiles[2].z)
                    {
                        draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up, nodeTiles[2] + Vector3.up, nodeTiles[2]));
                        draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[2], nodeTiles[2] + Vector3.up, nodeTiles[0] + Vector3.up));
                    }
                    else if (nodeTiles[1].x != nodeTiles[2].x && nodeTiles[1].z != nodeTiles[2].z)
                    {
                        draft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[1] + Vector3.up, nodeTiles[2] + Vector3.up, nodeTiles[2]));
                        draft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[2], nodeTiles[2] + Vector3.up, nodeTiles[1] + Vector3.up));
                    }
                    
                    // add wall
                }
                else if (nodeTiles.Count == 2)
                {
                    draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up, nodeTiles[1] + Vector3.up, nodeTiles[1]));
                    draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] + Vector3.up, nodeTiles[0] + Vector3.up));
                }
                    nodeTiles.Clear();
            }
        draft.Move(Vector3.left * center.x + Vector3.forward * center.z);
        m = draft.ToMesh();
        _target.mesh = m;
    }

    static Vector3 positionOf(int x, int y)
    {
        return Vector3.right * x + Vector3.back * (y);
    }
}
