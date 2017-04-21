using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestVoronoi : MonoBehaviour {

    public RawImage target;
    public int size = 512;
    public int Points = 30;
    public int distanceStart = 30;
    public int distanceEnd = 50;

	// Use this for initialization
	void Start () {
        
	}
	
	void generate()
    {
        var tex = new Texture2D(size, size);
        var points = new List<Vector2>();
        for (int i = 0; i < Points; i++)
        {
            points.Add(new Vector2(Random.Range(1, size), Random.Range(1, size)));
        }

        Color c = ProceduralToolkit.RandomE.color;

        for(int i = 0; i < size; i++)
            for(int j = 0; j < size; j++)
            {
                var coor = new Vector2(i, j);
                var nearest = points.OrderBy(m => Vector2.Distance(m, coor)).First();
                //tex.SetPixel(i,j, )
            }
    }
}
