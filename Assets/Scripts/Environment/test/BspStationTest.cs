using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BspStationTest : MonoBehaviour {

    private float timer = 0;
    private GameObject parent;

    public int iterations = 2;
    public int size = 70;
    public int minRoomSize = 8;
    public int halfCorridorSize = 1;
    public TileSet TileSet;
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if(timer > .5f)
        {
            timer = 0f;
            generate();
        }
	}

    void generate()
    {
        if (parent != null)
            Destroy(parent);

        parent = new GameObject();
        StationSpawner spawner = new StationSpawner();
        spawner.Generate(parent.transform, Random.Range(0, 1000), size, iterations, minRoomSize, halfCorridorSize, TileSet);
    }
}