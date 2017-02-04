using ProceduralToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour {

    public Vector2Int Size = new Vector2Int(6,14);
    int[,] tiles;
    bool changed;

    public MeshFilter Target;

    // Use this for initialization
    void Start()
    {
        tiles = new int[Size.x, Size.y];
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnGUI()
    {
        changed = false;
        if (GUILayout.Button("RESET"))
            reset();

        for (int y = 0; y < Size.y; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < Size.x; x++)
            {
                if (GUILayout.Button(tiles[x, y].ToString()))
                {
                    tiles[x, y] = tiles[x, y] == 1 ? 0 : 1;
                    changed = true;
                }
            }
            GUILayout.EndHorizontal();
        }

        if(changed)
        {
            reMesh();
        }
    }

    void reset()
    {
        tiles = new int[Size.x, Size.y];
        changed = true;
        for (int x = 0; x < Size.x; x++)
            for(int y = 0; y < Size.y; y++)
            {
                tiles[x,y] = 0;
            }
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
                    draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[2], nodeTiles[3]));
                    
                }
                else if (nodeTiles.Count == 3)
                {
                    draft.Add(MeshDraft.Triangle(nodeTiles[0], nodeTiles[1], nodeTiles[2]));

                    //draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[0] + Vector3.up, nodeTiles[1] + Vector3.up, nodeTiles[1]));
                    //draft.Add(MeshDraft.Quad(nodeTiles[0], nodeTiles[1], nodeTiles[1] + Vector3.up, nodeTiles[0] + Vector3.up));
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
        return Vector3.right * x + Vector3.forward * y;
    }
}
