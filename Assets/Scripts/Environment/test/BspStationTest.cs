using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class BspStationTest : MonoBehaviour {

    private float timer = 0;
    private GameObject parent;

    public bool autoCreate = false;
    public int iterations = 2;
    public int size = 70;
    public int minRoomSize = 8;
    public int halfCorridorSize = 1;
    public TileSet _tileset;
    public int seed = -1;
    List<int> sizes;
    public bool doStations = true;

    void Awake()
    {
        sizes = new List<int>() { 15, 20, 24 };
        StartCoroutine(SpawnRoutine());
    }
	
	// Update is called once per frame
	void Update () {
        if(SpawnQueue.Count == 0)
        timer += Time.deltaTime;
        if( Input.GetKeyDown(KeyCode.Space) || autoCreate && timer > .5f)
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
        autoCreate = GUILayout.Toggle(autoCreate, "Auto");
        doStations = GUILayout.Toggle(doStations, doStations ? "stations" : "Asteroids");

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
        else
        {
            GUILayout.Label("asteroid size array");
            for (int i = 0; i < sizes.Count; i++)
                sizes[i] = (int) GUILayout.HorizontalSlider((float)sizes[i], (float)(i == 0 ? 10 : sizes[i - 1]), (float)(i == sizes.Count - 1 ? 30 : sizes[i + 1]));
        }
    }

    void generateStation()
    {
        if (parent != null)
            Destroy(parent);

        InstantiateCount = 0;
        parent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        StationSpawner spawner = new StationSpawner();
        spawner.Tester = this;
        spawner.Generate(parent.transform, seed, size, iterations, minRoomSize, halfCorridorSize, _tileset);
    }

    void generateAsteroid()
    {
        if (parent != null)
            Destroy(parent);

        parent = new GameObject();
        AsteroidSpawner spawner = new AsteroidSpawner();
        
        spawner.DoAll(sizes, 1, parent.transform, seed, _tileset);
    }

    public Queue<SpawnCase> SpawnQueue = new Queue<SpawnCase>();
    public float BlockSize = 1f;
    public float SpawnFrameLimit = .2f;
    public int InstantiateCount = 0;
    public void Enqueue(Transform transform, Vector3 position, Quaternion q, Transform p)
    {
        SpawnQueue.Enqueue(new SpawnCase() { t = transform, position = position, Parent = p });
    }

    IEnumerator SpawnRoutine()
    {
        float timer = 0f;
        float real = 0f;
        while(true)
        {
            real = Time.realtimeSinceStartup;
            while (SpawnQueue.Count > 0 && timer < SpawnFrameLimit)
            {
                timer = (Time.realtimeSinceStartup - real);
                var c = SpawnQueue.Dequeue();
                var t = Instantiate(c.t, c.position * BlockSize, Quaternion.identity, c.Parent);
                t.localScale = Vector3.one * BlockSize;
                InstantiateCount++;
            }
            yield return new WaitForEndOfFrame();
            timer = 0f;
        }
    }
    public struct SpawnCase
    {
        public Transform t;
        public Vector3 position;
        public Transform Parent;
        public Vector3 Size;
    }
}