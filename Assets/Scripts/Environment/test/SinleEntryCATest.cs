using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SinleEntryCATest : MonoBehaviour {

    public List<Vector3> tiles = new List<Vector3>();
    public int Size = 20;
    public float SpawnPercentage = .6f;
    public int neightborMin = 5;

    List<Vector3> temp = new List<Vector3>();
    List<Vector3> tempout = new List<Vector3>();

    // Use this for initialization
    void Start () {
		
	}
	
	void OnGUI () {
        if (GUILayout.Button("Do noise"))
            createInitial();
        if (GUILayout.Button("smooth"))
            smooth();
	}

    void createInitial()
    {
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                {
                    if(Random.value > SpawnPercentage)
                    {
                        tiles.Add(new Vector3(i, j, k));
                    }
                }
    }

    void smooth()
    {
        temp = new List<Vector3>();
        //foreach(var v in tiles)
        foreach( var v in tiles)
        {
            if(!temp.Contains(v))
                checkPoint(v);
        }

        tiles = temp;
    }

    void checkPoint(Vector3 pos)
    {
        var neighbors = tiles.Count(m => (m - pos).magnitude <= 1f);
        if (neighbors > neightborMin)
            temp.Add(pos);
        else
            return;


        if (!temp.Contains(pos + Vector3.up))
            checkPoint(pos + Vector3.up );
        if (!temp.Contains(pos + Vector3.down))
            checkPoint(pos + Vector3.down );
        if (!temp.Contains(pos + Vector3.right))
            checkPoint(pos + Vector3.right );
        if (!temp.Contains(pos + Vector3.left))
            checkPoint(pos + Vector3.left);
        if (!temp.Contains(pos + Vector3.forward))
            checkPoint(pos + Vector3.forward);
        if (!temp.Contains(pos + Vector3.back))
            checkPoint(pos + Vector3.back);
    }

    void OnDrawGizmos()
    {
        foreach (var v in tiles)
            Gizmos.DrawCube(v, Vector3.one);

    }

    int neightborCount6(Vector3 pos)
    {
        //int x = (int)pos.x;
        //int y = (int)pos.y;
        //int z = (int)pos.z;
        /*
        var ret = 0;
        for (int i = x - 1; i <= x + 1; i++)
            for (int j = y - 1; j <= y + 1; j++)
                for (int k = z - 1; k <= z + 1; k++)
                {
                    if (inBounds(i, j, k))//i >= 0 && i < Size && j >= 0 && j < Size && k >= 0 && k < Size)
                        ret += map[i, j, k];
                }
        */
        var ret = 0;
        ret = tiles.Count(m => (m - pos).magnitude <= 1f);

        return ret;
    }
}
