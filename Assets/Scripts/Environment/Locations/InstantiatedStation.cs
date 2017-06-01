using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedStation : InstantiatedLocation {

    public Duplicate BaseBlock;
    int[,,] voxelMap;
    Dictionary<Vector3, Material> materialOverview = new Dictionary<Vector3, Material>();

    // Use this for initialization
    void Start() {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void BlockHit(int x, int y, int z)
    {
        //base.BlockHit(x, y, z);
        Vector3 pos = new Vector3(x , y, z) + transform.position;
        DrawInstancedIndirect.RemoveFromDraw(materialOverview[pos], pos);
        materialOverview.Remove(pos);
    }

    public void Spawn(int[,,] map)
    {
        voxelMap = map;
        StartCoroutine(spawnMyBlocks());
        createBlocks(transform);
    }

    void createBlocks(Transform parent)
    {
        var set = Resources.LoadAll<LocationTileSet>("TileSets").GetRandom();
        
        for (int x = 0; x < voxelMap.GetLength(0); x++)
            for (int y = 0; y < voxelMap.GetLength(1); y++)
                for (int z = 0; z < voxelMap.GetLength(2); z++)
                {
                    if (voxelMap[x, y, z] > 0)
                    {
                        Vector3 pos = new Vector3(x , y, z) + transform.position;

                        materialOverview.Add(pos, set.Materials[voxelMap[x, y, z]]);
                        DrawInstancedIndirect.AddToDraw(set.Materials[voxelMap[x,y,z]], pos, 8);
                        //DrawInstanced.Instance.AddToDraw(transform, set.GroundTiles[voxelMap[x, y, z] - 1].GetComponent<MeshRenderer>().material);
                        //CoroutineSpawner.Instance.Enqueue(set.GroundTiles[voxelMap[x, y, z] - 1], Vector3.right * x + Vector3.forward * z + Vector3.up * (y - 2), Quaternion.identity, parent.transform);
                        blocksToSpawn.Enqueue(new SpawnCase( 8, x,y,z));
                    }
                }
    }

    Queue<SpawnCase> blocksToSpawn = new Queue<SpawnCase>();
    float spawnTime = .1f;
    IEnumerator spawnMyBlocks()
    {
        BaseBlock = Resources.Load<Duplicate>("BaseBlock");
        float real = 0f;
        float timer = 0f;
        while (gameObject.activeInHierarchy)
        {
            real = Time.realtimeSinceStartup;
            while(blocksToSpawn.Count > 0 && timer < spawnTime)
            {
                timer = (Time.realtimeSinceStartup - real);
                var c = blocksToSpawn.Dequeue();
                var t = Instantiate(BaseBlock, transform);
                t.gameObject.layer = c.Layer;
                t.transform.localPosition = new Vector3(c.X, c.Y, c.Z);

                t.Owner = this;
                t.X = c.X;
                t.Y = c.Y;
                t.Z = c.Z;
            }
            yield return new WaitForEndOfFrame();
            timer = 0f;
        }
    }

    public struct SpawnCase
    {
        public SpawnCase( int layer, int x, int y, int z)
        {
            Layer = layer;
            X = x;
            Y = y;
            Z = z;
        }
        public int Layer;
        public int X;
        public int Y;
        public int Z;
    }
}
