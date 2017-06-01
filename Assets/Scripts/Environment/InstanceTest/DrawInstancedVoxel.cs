using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawInstancedVoxel : MonoBehaviour {

    private static DrawInstancedVoxel instance;
    public static DrawInstancedVoxel Instance
    {
        get
        {
            if (instance == null)
                instance = new GameObject("GpuInstance").AddComponent<DrawInstancedVoxel>();
            return instance;
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
