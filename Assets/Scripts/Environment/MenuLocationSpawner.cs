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

    public int SpawnCount = 5;
    List<int> sizes;
    bool working = false;

    // Use this for initialization
    void Start () {
        timer = 0f;
    }

    // Update is called once per frame
    void Update () {
        //timer += Time.deltaTime;
        //if (autoCreate && timer > 4f )
        //{
        //    DrawInstancedIndirect.RemoveAll();
        //    StopAllCoroutines();
        //    timer = 0f;
        //    generateStation();
        //}

        if(!working)
        {
            StopAllCoroutines();
            DrawInstancedIndirect.RemoveAll();
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
        var map = spawner.Generate(transform, seed, size, iterations, minRoomSize, halfCorridorSize, _tileset, false);
        var vox = spawner.ToVoxelMap(10);
        StartCoroutine(createBlocks(vox));
    }

    public Dictionary<Vector3,Material> blocks = new Dictionary<Vector3, Material>();
    public int count = 0;

    IEnumerator createBlocks(int[,,] voxelMap)
    {
        working = true;
        var set = Resources.LoadAll<LocationTileSet>("TileSets").GetRandom();
         //count = 0;
        for (int x = 0; x < voxelMap.GetLength(0); x++)
            for (int y = 0; y < voxelMap.GetLength(1); y++)
                for (int z = 0; z < voxelMap.GetLength(2); z++)
                {
                    if (voxelMap[x, y,z] > 0)
                    {
                        Vector3 pos = new Vector3(x, y, z) + transform.position;
                        blocks.Add(pos, set.Materials[voxelMap[x, y, z]]);

                        //DrawInstanced.Instance.AddToDraw(transform, set.GroundTiles[voxelMap[x, y, z] - 1].GetComponent<MeshRenderer>().material);
                        //CoroutineSpawner.Instance.Enqueue(set.GroundTiles[voxelMap[x, y, z] - 1], Vector3.right * x + Vector3.forward * z + Vector3.up * (y - 2), Quaternion.identity, parent.transform);

                    }
                }

        while(blocks.Count > 0)
        {
            var t = blocks.GetRandom();
            DrawInstancedIndirect.AddToDraw(t.Value, t.Key, 8);
            blocks.Remove(t.Key);
            count++;
            if (count % SpawnCount == 0)
                yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3f);
        working = false;
    }
}
