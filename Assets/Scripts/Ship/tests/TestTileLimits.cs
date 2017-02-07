using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestTileLimits : MonoBehaviour {
    public int Size = 5;
    int[,] map;
    List<float> angles = new List<float>();
    float limitYMin;
    float limitYMax;
	// Use this for initialization
	void Start () {
        map = new int[Size, Size];
        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                map[i, j] = 1;
    }
	
	// Update is called once per frame
	void OnGUI () {
        GUILayout.Label(limitYMin + " . " + limitYMax);
        for(int j = Size-1; j >= 0 ; j--)
            {
            GUILayout.BeginHorizontal();
		    for(int i = 0; i < Size; i++)
            {
                if (GUILayout.Button(map[i,j].ToString()))
                {
                    map[i, j] = map[i, j] == 0 ? 1 : 0;
                    calcAngles(2, 2);
                }
            }
            GUILayout.EndHorizontal();
        }

        foreach (var a in angles)
            GUILayout.Label(a.ToString());
	}

    void calcAngles(int x, int y)
    {
        angles = new List<float>();
        for (int i = -1; i <=  1; i++)
            for (int j = - 1; j <= 1; j++)
            {
                if (map[i + x, j + y] == 0)
                {
                    var norm = Vector3.right * (i) + Vector3.forward * (j);
                    norm.Normalize();
                    var ang = Vector3.Angle(Vector3.forward, norm);
                    var cross = Vector3.Cross(Vector3.forward, norm);
                    if (cross.y < 0)
                        ang = -ang;
                    //Debug.Log("V3 angle : " + Vector3.Angle(Vector3.forward, norm));
                    //Debug.Log("cos of " + norm.z + " = " + Mathf.Acos(norm.z) * Mathf.Rad2Deg);
                    //Debug.Log("sin of " + norm.x + " = " + Mathf.Asin(norm.x) * Mathf.Rad2Deg);
                    angles.Add(ang);
                }
            }
        //angles.Sort();
        limitYMin = angles.Min();
        limitYMax = angles.Max();
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
            {
                if(map[i,j] == 1) 
                    Gizmos.DrawCube(Vector3.right * i + Vector3.forward * j, Vector3.one * .9f);
            }

        for(int i = 1; i <= 3; i++)
            for(int j = 1; j <= 3; j++)
            {
                if (map[i,j] == 0)
                {
                    Gizmos.DrawLine(Vector3.right * 2 + Vector3.forward * 2, Vector3.right * i + Vector3.forward * j);
                }
            }
    }
}
