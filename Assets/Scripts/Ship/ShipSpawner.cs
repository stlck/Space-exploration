﻿using ProceduralToolkit;
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
    Transform testControls;
    Rect pos;
    // Use this for initialization
    public void Start()
    {
        reset();

        ControlList = Resources.LoadAll<GameObject>("ShipControls").ToList();
        pos = new Rect(300, 100, 400, 500);

        if (ControlList.Any())
            currentControl = 1;
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

    public static void FromClientCreateShip(string shipString, string controlString, int sizex, int sizey, Vector3 pos, Quaternion rotation)
    {
        int[] t = new int[shipString.Length];
        var tiles = new int[sizex, sizey];
        for (int i = 0; i < shipString.Length; i++)
            t[i] = (int)char.GetNumericValue(shipString[i]);

        int[] c = new int[controlString.Length];
        var controls = new int[sizex, sizey];
        for (int i = 0; i < controlString.Length; i++)
            c[i] = (int)char.GetNumericValue(controlString[i]);


        int counter = 0;

        for (int i = 0; i < sizex; i++)
            for (int j = 0; j < sizey; j++)
            {
                tiles[i, j] = t[counter];
                controls[i, j] = c[counter];
                counter++;
            }

        SrvCreateShip(tiles, controls, sizex, sizey, pos, rotation);
    }

    public static void SrvCreateShip(int[,] t, int[,] c, int sizex, int sizey, Vector3 position, Quaternion rotation)
    {
        var baseShip = Resources.Load<Ship>("BaseShip");
        var s = Instantiate(baseShip, Vector3.left * 10f, Quaternion.identity);//Target.gameObject.AddComponent<Ship>();
        
        NetworkServer.Spawn(s.gameObject);
        var center = new Vector3(sizex / 2, 0, sizey / 2);

        for (int y = 1; y < sizey - 1; y++)
            for (int x = 1; x < sizex - 1; x++)
                if (c[x, y] >= 1)
                {
                    var spawn = new NetworkSpawnObject();
                    spawn.SpawnTarget = ControlList[c[x, y] -1];
                    spawn.Position = positionOf(x, y) + Vector3.left * center.x + Vector3.forward * center.z;
                    spawn.PositionIsLocal = true;
                    spawn.Parent = s.gameObject;
                    s.NetworkSpawnObjects.Add(spawn);
                    //NetworkSpawnObjects.ForEach(m => NetworkHelper.Instance.NetworkSpawnObject(m));
                    var spawned = NetworkHelper.Instance.NetworkSpawnObject(spawn);
                    spawned.GetComponent<IShipSpawnObject>().SetTilePosition(new Vector2Int(x, y));
                }

        s.transform.position = position;
        s.transform.rotation = rotation;

        s.tiles = t;
        s.Sizex = sizex;
        s.Sizey = sizey;
    }

    void buildWindow(int id)
    {
        changed = false;

        if (GUILayout.Button("RESET"))
            reset();

        if (GUILayout.Button("Cmd! Create Ship")) { 
            if(MyAvatar.Instance.isServer)
            {
                var ctrlString = "";
                for (int row = 0; row < Size.x; row++)
                    for (int col = 0; col < Size.y; col++)
                    {
                        if (controls[row, col] >= 0)
                            ctrlString += controls[row, col];
                        else
                            ctrlString += "-";
                    }
                Debug.Log(ctrlString);
                int[] c = new int[ctrlString.Length];
                string output = "";
                for (int i = 0; i < ctrlString.Length; i++)
                {
                    c[i] = (int)char.GetNumericValue(ctrlString[i]);
                    output += c[i] + "";
                }
                Debug.Log(output);

                SrvCreateShip(tiles, controls, Size.x, Size.y, TargetDock.transform.position, TargetDock.DockAlign.rotation);//createShip();
            }
            else
            {
                var shipString = "";
                var ctrlString = "";
                for(int row =0; row < Size.x;row++)
                    for(int col = 0; col < Size.y; col++)
                    {
                        if (controls[row, col] >= 0)
                            ctrlString += controls[row, col];
                        else
                            ctrlString += "-";
                        shipString += tiles[row, col];
                    }

                MyAvatar.Instance.MyCommands.CmdSpawnShip(shipString, ctrlString, Size.x, Size.y, TargetDock.DockAlign.position, TargetDock.DockAlign.rotation);
            }
        }

        if (GUILayout.Button("test create ship2"))
            createShipTest();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        showLayout();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        showCommandObjects();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        
        if (IsTesting && changed)
        {
            //foreach (Transform c in Target.transform)
            //    Destroy(c.gameObject);
            ShipToMesh(Target.transform, Size.x, Size.y, tiles, Target.GetComponent<MeshRenderer>().material);
            showControls();
        }

        GUI.DragWindow();
    }

    void showLayout()
    {
        //GUILayout.BeginArea(new Rect(10, 10, pos.width / 2 - 20, 300));
        for (int y = 1; y < Size.y - 1; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 1; x < Size.x - 1; x++)
            {
                if (y == center.z && x == center.x)
                    GUI.contentColor = Color.green;
                else if (tiles[x, y] > 0)
                    GUI.contentColor = Color.white;
                else
                    GUI.contentColor = Color.grey;

                if (GUILayout.Button(tiles[x, y].ToString(), GUILayout.Width(22)) && hasNeighbor(x, y) > 0)
                {
                    tiles[x, y] = tiles[x, y] == 1 ? 0 : 1;
                    if (tiles[x, y] == 0)
                        controls[x, y] = -1;
                    changed = true;
                }
            }
            GUILayout.EndHorizontal();
        }
        //GUILayout.EndArea();
    }

    void showCommandObjects()
    {
        //GUILayout.BeginArea(new Rect(10, pos.width / 2 + 10, pos.width / 2 - 20, 300));
        int index = 1;
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
            for (int x = 1; x < Size.x - 1; x++)
            {
                if (tiles[x, y] == 1)
                {
                    //if(hasNeighbor(x,y) == 4)
                    if (matchConfig(ControlList[currentControl - 1], x, y))
                    {
                        if (controls[x, y] == -1)
                            GUI.contentColor = Color.green;
                        else
                            GUI.contentColor = Color.white;

                        if (controls[x, y] == -1 && GUILayout.Button("x", GUILayout.Width(25)))// && hasNeighbor(x, y) > 2)
                        {
                            controls[x, y] = currentControl;//ControlList.IndexOf(currentControl);
                            changed = true;
                        }
                        else if (controls[x, y] >= 0 && GUILayout.Button(controls[x, y].ToString(), GUILayout.Width(25)))
                        {
                            controls[x, y] = -1;
                            changed = true;
                        }
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
        //GUILayout.EndArea();
    }

    public void OnGUI()
    {
        if (!Show) return;

        pos = GUILayout.Window(41, pos, buildWindow, "SHIP");
    }

    bool matchConfig(GameObject go, int x, int y)
    {
        var spawnobj = go.GetComponent<IShipSpawnObject>();
        var config = spawnobj.TileConfig();

        if (config.Count == 0)
            return true;

        foreach (var c in config)
            if (tiles[x + c.x, y + c.y] == 0)
                return true;
                
        return false;
    }

    void createShipTest()
    {
        ShipToMesh(Target.transform, Size.x, Size.y, tiles, Target.GetComponent<MeshRenderer>().material);
        
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

        for(int i = (int)center.x -2; i <= center.x + 2; i++)
            for(int j = (int)center.z - 2; j <= center.z + 2; j++)
            {
                tiles[i, j] = 1;
            }
    }

    void showControls()
    {
        if(testControls == null)
        {
            testControls = new GameObject("testControls").transform;
            testControls.SetParent(Target.transform.root);
        }

        while(testControls.childCount > 0)
            DestroyImmediate(testControls.GetChild(0).gameObject);

        for (int y = 1; y < Size.y - 1; y++)
            for (int x = 1; x < Size.x - 1; x++)
            {
                if(controls[x,y] >= 0)
                {
                    var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    c.transform.SetParent(testControls);
                    c.transform.localPosition = positionOf(x, y) + Vector3.left * center.x + Vector3.forward * center.z;
                }
            }
    }

    public static void ShipToMesh(Transform _target, int _sizex, int _sizey, int[,] _tiles, Material mat = null)
    {
        Debug.Log("CREATING SHIP");
        var center = new Vector3((int)_sizex / 2, 0, (int)_sizey / 2);
        MeshDraft lowerDraft = new MeshDraft();
        //MeshDraft upperDraft = new MeshDraft();
        Mesh lower = new Mesh();
        //Mesh upper = new Mesh();

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

                // floor and roof
                if (nodeTiles.Count == 4)
                {
                    lowerDraft.Add(MeshDraft.Quad(nodeTiles[3], nodeTiles[2], nodeTiles[1], nodeTiles[0]));
                    //upperDraft.Add(MeshDraft.Quad(nodeTiles[3] + Vector3.up * 2, nodeTiles[2] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));
                    
                }
                // curved side
                else if (nodeTiles.Count == 3)
                {
                    lowerDraft.Add(MeshDraft.Triangle(nodeTiles[2], nodeTiles[1], nodeTiles[0]));
                    //upperDraft.Add(MeshDraft.Triangle(nodeTiles[2] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));
                    
                    if (nodeTiles[0].x != nodeTiles[1].x && nodeTiles[0].z != nodeTiles[1].z)
                    {
                        //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[1]));
                        //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));

                        lowerDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] - Vector3.up * 2, nodeTiles[1] - Vector3.up * 2, nodeTiles[1]));
                        lowerDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] - Vector3.up * 2, nodeTiles[0] - Vector3.up * 2));
                    }
                    else if (nodeTiles[0].x != nodeTiles[2].x && nodeTiles[0].z != nodeTiles[2].z)
                    {
                        //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up * 2, nodeTiles[2] + Vector3.up * 2, nodeTiles[2]));
                        //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[2], nodeTiles[2] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));

                        lowerDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] - Vector3.up * 2, nodeTiles[2] - Vector3.up, nodeTiles[2]));
                        lowerDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[2], nodeTiles[2] - Vector3.up * 2, nodeTiles[0] - Vector3.up * 2));
                    }
                    else if (nodeTiles[1].x != nodeTiles[2].x && nodeTiles[1].z != nodeTiles[2].z)
                    {
                        //upperDraft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[1] + Vector3.up * 2, nodeTiles[2] + Vector3.up * 2, nodeTiles[2]));
                        //upperDraft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[2], nodeTiles[2] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2));

                        lowerDraft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[1] - Vector3.up * 2, nodeTiles[2] - Vector3.up * 2, nodeTiles[2]));
                        lowerDraft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[2], nodeTiles[2] - Vector3.up * 2, nodeTiles[1] - Vector3.up * 2));
                    }
                    
                    // add wall
                }

                // straight side
                else if (nodeTiles.Count == 2)
                {
                    //var dir = nodeTiles[0].x - _sizex / 2;
                    //dir = Mathf.Clamp(dir, -1, 1);
                    //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[1]));
                    //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));

                    lowerDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] - Vector3.up * 2, nodeTiles[1] - Vector3.up * 2, nodeTiles[1]));
                    lowerDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] - Vector3.up * 2, nodeTiles[0] - Vector3.up * 2));
                }
                nodeTiles.Clear();
            }

        lowerDraft.Move(Vector3.left * center.x + Vector3.forward * center.z);
        lower = lowerDraft.ToMesh();


        var lChild = _target.Find("Bottom").gameObject;
        //lChild.transform.SetParent(_target);
        //lChild.transform.localPosition = Vector3.zero;
        //lChild.transform.localScale = Vector3.one;
        lChild.GetComponent<MeshFilter>().mesh = lower;
        lChild.GetComponent<MeshRenderer>().material = mat;//_target.GetComponent<MeshRenderer>().material;
        //lChild.gameObject.layer = LayerMask.NameToLayer("Ship");
        var coll = lChild.gameObject.AddComponent<MeshCollider>();
        coll.sharedMesh = lower;
        coll.GetComponent<MeshCollider>().convex = true;
        //lChild.GetComponent<MeshCollider>().sharedMesh = lower;
        //lChild.GetComponent<MeshCollider>().convex = true;


        Mesh upper = new Mesh();
        MeshDraft upperDraft = new MeshDraft();
        tileCode code = 0;
        for (int x = 0; x < _sizex - 1; x++)
            for (int y = 0; y < _sizey - 1; y++)
            {
                code = tileCode.none;
                if (_tiles[x, y] > 0)
                {
                    nodeTiles.Add(positionOf(x, y));
                    code |= tileCode.lowerLeft;
                }
                if (_tiles[x, y + 1] > 0)
                {
                    nodeTiles.Add(positionOf(x, y + 1));
                    code |= tileCode.upperleft;
                }
                if (_tiles[x + 1, y + 1] > 0)
                {
                    nodeTiles.Add(positionOf(x + 1, y + 1));
                    code |= tileCode.upperright;
                }
                if (_tiles[x + 1, y] > 0)
                {
                    nodeTiles.Add(positionOf(x + 1, y));
                    code |= tileCode.lowerright;
                }

                // floor and roof
                if (nodeTiles.Count == 4)
                {
                    upperDraft.Add(MeshDraft.Quad(nodeTiles[3] + Vector3.up * 2, nodeTiles[2] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));

                }
                // curved side
                else if (nodeTiles.Count == 3)
                {
                    upperDraft.Add(MeshDraft.Triangle(nodeTiles[2] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));
                    if ((code & tileCode.lowerLeft) != tileCode.lowerLeft)
                    {
                        upperDraft.Add(MeshDraft.Triangle(nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.forward, nodeTiles[2] + Vector3.up * 2));

                    }
                    else if ((code & tileCode.upperleft) != tileCode.upperleft)
                    {
                        upperDraft.Add(MeshDraft.Triangle(nodeTiles[0] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.back));
                    }
                    else if ((code & tileCode.lowerright) != tileCode.lowerright)
                    {
                        upperDraft.Add(MeshDraft.Triangle(nodeTiles[0] + Vector3.up * 2,  nodeTiles[0] + Vector3.right, nodeTiles[2] + Vector3.up * 2));

                    }
                    else if ((code & tileCode.upperright) != tileCode.upperright)
                    {
                        upperDraft.Add(MeshDraft.Triangle(nodeTiles[1] + Vector3.up * 2, nodeTiles[2] + Vector3.up * 2, nodeTiles[1] + Vector3.right));
                    }
                    /*
                                        if (nodeTiles[0].x != nodeTiles[1].x && nodeTiles[0].z != nodeTiles[1].z)
                                        {
                                            upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[1]));
                                            upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));
                                        }
                                        else if (nodeTiles[0].x != nodeTiles[2].x && nodeTiles[0].z != nodeTiles[2].z)
                                        {
                                            upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up * 2, nodeTiles[2] + Vector3.up * 2, nodeTiles[2]));
                                            upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[2], nodeTiles[2] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));
                                        }
                                        else if (nodeTiles[1].x != nodeTiles[2].x && nodeTiles[1].z != nodeTiles[2].z)
                                        {
                                            upperDraft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[1] + Vector3.up * 2, nodeTiles[2] + Vector3.up * 2, nodeTiles[2]));
                                            upperDraft.Add(MeshDraft.Quad(nodeTiles[1], nodeTiles[2], nodeTiles[2] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2));
                                        }*/

                    // add wall
                }

                // straight side
                else if (nodeTiles.Count == 2)
                {
                    if((code & tileCode.lowerLeft) == tileCode.lowerLeft && (code & tileCode.upperleft) == tileCode.upperleft)
                    {
                        upperDraft.Add(MeshDraft.Quad(nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.right, nodeTiles[1] + Vector3.right));
                    }
                    else if((code & tileCode.lowerright) == tileCode.lowerright && (code & tileCode.upperright) == tileCode.upperright)
                    {
                        upperDraft.Add(MeshDraft.Quad(nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.left, nodeTiles[1] + Vector3.left));

                    }
                    else if ((code & tileCode.lowerLeft) == tileCode.lowerLeft && (code & tileCode.lowerright) == tileCode.lowerright)
                    {
                        upperDraft.Add(MeshDraft.Quad( nodeTiles[1] + Vector3.back, nodeTiles[0] + Vector3.back, nodeTiles[0] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2));
                    }
                    else if ((code & tileCode.upperleft) == tileCode.upperleft && (code & tileCode.upperleft) == tileCode.upperleft)
                    {
                        upperDraft.Add(MeshDraft.Quad(nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.forward, nodeTiles[1] + Vector3.forward));
                    }
                    //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up * 2, nodeTiles[1] + Vector3.up * 2, nodeTiles[1]));
                    //upperDraft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] + Vector3.up * 2, nodeTiles[0] + Vector3.up * 2));
                }
                else if(nodeTiles.Count == 1)
                {
                    switch(code)
                    {
                        case tileCode.lowerLeft:
                            upperDraft.Add(MeshDraft.Triangle(nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.right, nodeTiles[0] + Vector3.back));
                            break;
                        case tileCode.upperleft:
                            upperDraft.Add(MeshDraft.Triangle(nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.forward, nodeTiles[0] + Vector3.right));
                            break;
                        case tileCode.upperright:
                            upperDraft.Add(MeshDraft.Triangle(nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.left, nodeTiles[0] + Vector3.forward));
                            break;
                        case tileCode.lowerright:
                            upperDraft.Add(MeshDraft.Triangle(nodeTiles[0] + Vector3.up * 2, nodeTiles[0] + Vector3.back, nodeTiles[0] + Vector3.left));
                            break;
                        default:
                            break;
                    }
                }
                nodeTiles.Clear();
            }

        upperDraft.Move(Vector3.left * center.x + Vector3.forward * center.z);
        upper = upperDraft.ToMesh();

        var uChild = _target.Find("Top").gameObject;
        //uChild.transform.SetParent(_target);
        //uChild.transform.localPosition = Vector3.zero;
        //uChild.transform.localScale = Vector3.one;
        uChild.GetComponent<MeshFilter>().mesh = upper;
        uChild.GetComponent<MeshRenderer>().material = mat;// _target.GetComponent<MeshRenderer>().material;
        uChild.gameObject.layer = LayerMask.NameToLayer("ShipTop");
        var coll2 = uChild.gameObject.AddComponent<MeshCollider>();
        coll2.sharedMesh = upper;
        coll2.GetComponent<MeshCollider>().convex = true;
        //uChild.GetComponent<MeshCollider>().sharedMesh = upper;
        //uChild.GetComponent<MeshCollider>().convex = true;
    }

    static Vector3 positionOf(int x, int y)
    {
        return Vector3.right * x + Vector3.back * (y);
    }

    [System.Flags]
    enum tileCode
    {
        none = 0,
        lowerLeft = 1,
        lowerright = 2,
        upperleft = 4,
        upperright = 8
    }
}
