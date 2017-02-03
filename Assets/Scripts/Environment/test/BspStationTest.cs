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
    public TileSet _tileset;
    public int seed = -1;

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
        doStations = GUILayout.Toggle(doStations, doStations ? "stations" : "Asteroids");

        GUILayout.Label("Size : " + size);
        size = (int)GUILayout.HorizontalSlider((float)size, 0f, 100f);

        GUILayout.Label("Size : " + size);
        size = (int)GUILayout.HorizontalSlider((float)size, 0f, 100f);

        //GUILayout.Label("TileSet : " + _tileset);
        //foreach (var t in System.Enum.GetValues(typeof(TileSet)))
        //    if (GUILayout.Button(t.ToString()))
        //        _tileset = t;

        if (doStations)
        {
            GUILayout.Label("Splits : " + iterations);
            iterations = (int)GUILayout.HorizontalSlider((float)iterations, 1f, 8f);

            GUILayout.Label("Min Room Size : " + minRoomSize);
            minRoomSize = (int)GUILayout.HorizontalSlider((float)minRoomSize, 4f, 20f);

            GUILayout.Label("Half corridor size : " + halfCorridorSize);
            halfCorridorSize = (int)GUILayout.HorizontalSlider((float)halfCorridorSize, 1f, 4f);
        }
    }

    void generateStation()
    {
        if (parent != null)
            Destroy(parent);

        parent = new GameObject();
        StationSpawner spawner = new StationSpawner();
        spawner.Generate(parent.transform, seed, size, iterations, minRoomSize, halfCorridorSize, _tileset);
    }

    void generateAsteroid()
    {
        if (parent != null)
            Destroy(parent);

        parent = new GameObject();
        AsteroidSpawner spawner = new AsteroidSpawner();
        var sizes = new List<int>() { 15, 20, 24 };
        spawner.DoAll(sizes, 1, parent.transform, seed, _tileset);
    }
}