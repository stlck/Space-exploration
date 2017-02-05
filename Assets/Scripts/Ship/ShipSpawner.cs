using ProceduralToolkit;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipSpawner : MonoBehaviour {

    public Vector2Int Size = new Vector2Int(11,14);
    int[,] tiles;
    int[,] controls;
    bool changed;

    bool doGround = true;
    bool showBuilder = true;

    public MeshFilter Target;
    public List<GameObject> ControlList = new List<GameObject>();
    int currentControl;
    Vector3 center;
    Vector2 scrollPosition;

    // Use this for initialization
    public void Start()
    {
        reset();
        if (ControlList.Any())
            currentControl = 0;
    }

    public void OnGUI()
    {
        if (!showBuilder) return;

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        changed = false;
        if (GUILayout.Button("RESET"))
            reset();
        if (GUILayout.Button("Create Ship"))
            createShip();

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
                if (GUILayout.Button(index + ":\t" + c.name))
                    currentControl = index++;
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
        if (changed)
        {
            reMesh();
        }
    }

    void createShip()
    {
        reMesh();
        var baseShip = Resources.Load<Ship>("BaseShip");
        var s = Instantiate(baseShip, Vector3.zero, Quaternion.identity);//Target.gameObject.AddComponent<Ship>();
        var mTarget = s.GetComponentInChildren<MeshFilter>();
        mTarget.mesh = Target.mesh;
        mTarget.gameObject.AddComponent<MeshCollider>();

        for (int y = 1; y < Size.y - 1; y++)
            for (int x = 1; x < Size.x-1; x++)
                if(controls[x,y] >= 0)
                {
                    var spawn = new NetworkSpawnObject();
                    spawn.SpawnTarget = ControlList[controls[x, y]];
                    spawn.Position = positionOf(x, y);
                    spawn.PositionIsLocal = true;
                    spawn.Parent = Target.gameObject;
                    s.NetworkSpawnObjects.Add(spawn);
                }
                    
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

    void reMesh()
    {
        MeshDraft draft = new MeshDraft();
        Mesh m = new Mesh();
        List<Vector3> nodeTiles = new List<Vector3>();
        for (int x = 0; x < Size.x-1; x++)
            for (int y = 0; y < Size.y-1; y++)
            {
                if (tiles[x, y ] > 0)
                    nodeTiles.Add(positionOf(x, y));
                if (tiles[x, y+1] > 0)
                    nodeTiles.Add(positionOf(x, y+1));
                if (tiles[x+1, y + 1] > 0)
                    nodeTiles.Add(positionOf(x+1, y + 1));
                if (tiles[x+1, y] > 0)
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

        m = draft.ToMesh();
        Target.mesh = m;
    }

    Vector3 positionOf(int x, int y)
    {
        return Vector3.right * x + Vector3.forward * (Size.y - y) - center;
    }
}
