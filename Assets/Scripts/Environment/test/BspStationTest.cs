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

    public bool doStations = true;
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if( Input.GetKeyDown(KeyCode.Space))//timer > .5f)
        {
            timer = 0f;
            if (doStations)
                generateStation();
            else
                generateAsteroid();
        }
	}

    void OnGUI()
    {
        doStations = GUILayout.Toggle(doStations, doStations ? "Asteroids" : "stations");
        GUILayout.Label("Size : " + size);
        size = (int)GUILayout.HorizontalSlider((float)size, 0f, 100f);

        if(doStations)
        {
            GUILayout.Label("Splits : " + iterations);
            iterations = (int)GUILayout.HorizontalSlider((float)iterations, 1f, 8f);
        }
    }

    void generateStation()
    {
        if (parent != null)
            Destroy(parent);

        parent = new GameObject();
        StationSpawner spawner = new StationSpawner();
        spawner.Generate(parent.transform, Random.Range(0, 1000), size, iterations, minRoomSize, halfCorridorSize, TileSet);
    }

    void generateAsteroid()
    {
        if (parent != null)
            Destroy(parent);

        parent = new GameObject();
        AsteroidSpawner spawner = new AsteroidSpawner();
        var sizes = new List<int>() { 15, 20, 24 };
        spawner.DoAll(sizes, 1, parent.transform);
    }
}