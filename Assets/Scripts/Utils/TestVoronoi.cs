using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TestVoronoi : MonoBehaviour {

    public RawImage target;
    public int size = 512;
    public int Points = 30;
    public float distanceStart = 30;
    public float distanceEnd = 50;
    public Gradient grad;

	// Use this for initialization
	void Start () {
        generate();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            generate();
    }

    void generate ()
    {
    }
}
