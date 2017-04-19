using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLocationSpawner : MonoBehaviour {

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

    public CoroutineSpawner spawner;

    // Use this for initialization
    void Start () {
        timer = 2f;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (autoCreate && timer > 2f && spawner.SpawnQueue.Count == 0)
        {
            timer = 0f;
            generateStation();
        }
    }

    void generateStation ()
    {
        if (parent != null)
            Destroy(parent);

        parent = new GameObject();
        StationSpawner spawner = new StationSpawner();
        _tileset = (TileSet)Random.Range(0, 4);
        spawner.Generate(parent.transform, seed, size, iterations, minRoomSize, halfCorridorSize, _tileset);
    }


}
