using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTest : MonoBehaviour {

    public GameObject NoNeighbors;
    public GameObject OneNeighbors;
    public GameObject TwoStraightNeighbors;
    public GameObject TwoCornerNeighbors;
    public GameObject ThreeNeighbors;
    public GameObject FourNeighbors;

    public WallObject[,] TileMap;
    public int size;
    Transform parent;
    // Use this for initialization
    void Start () {
        StartCoroutine(CreateWalls());


    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(CreateWalls());

    }

    private void OnGUI ()
    {
        size = (int)GUILayout.HorizontalSlider(size, 20, 100, GUILayout.Width(200));
    }

    IEnumerator CreateWalls()
    {
        var time = Time.realtimeSinceStartup;
        Debug.Log("TIme start");
        done = false;
        if (parent != null)
            Destroy(parent.gameObject);
        parent = new GameObject("p").transform;
        TileMap = new WallObject[size, size];
        var stationSpawner = new StationSpawner();
        var tiles = stationSpawner.Generate(parent, Random.Range(0, 10000), size, 5, 8, 1, TileSet.BlackAsteroid,false);
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                TileMap[i, j].WallValue = tiles[i, j];
                /*  if (i == 2 || i == 9)
                      TileMap[i, j].WallValue = 1;
                  else if (j == 2 || j == 3 || j == 8)
                      TileMap[i, j].WallValue = 1;*/
            }

        // calc neighbors
        for (int x = 0; x < size; x++)
            for (int z = 0; z < size; z++)
            {
                TileMap[x, z].NeighborRight = x < size - 1  && TileMap[x + 1, z ].WallValue == 2;
                TileMap[x, z].NeighborLeft = x > 0 && TileMap[x - 1, z ].WallValue == 2;
                TileMap[x, z].NeighborBack = z > 0 && TileMap[x, z-1].WallValue == 2;
                TileMap[x, z].NeighborForward = z < size -1 && TileMap[x, z+1].WallValue == 2;

                TileMap[x, z].NeighborCount = 0;
                if (TileMap[x, z].NeighborRight)
                    TileMap[x, z].NeighborCount++;
                if (TileMap[x, z].NeighborLeft)
                    TileMap[x, z].NeighborCount++;
                if (TileMap[x, z].NeighborForward)
                    TileMap[x, z].NeighborCount++;
                if (TileMap[x, z].NeighborBack)
                    TileMap[x, z].NeighborCount++;
            }

        yield return new WaitForSeconds( .1f);

        for (int x = 0; x < size; x++)
            for (int z = 0; z < size; z++)
            {
                if (TileMap[x, z].WallValue == 1)
                    Instantiate(FourNeighbors, Vector3.forward * z + Vector3.down + Vector3.right * x, Quaternion.identity, parent);
                if (TileMap[x, z].WallValue == 2)
                    {
                    Instantiate(FourNeighbors, Vector3.forward * z + Vector3.down + Vector3.right * x, Quaternion.identity, parent);
                    switch(TileMap[x, z].NeighborCount)
                    {
                        case 0:
                            instantiateWall(NoNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.identity, parent);
                            break;
                        case 1:
                            if(TileMap[x, z].NeighborForward)
                                instantiateWall(OneNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.identity, parent);
                            else if(TileMap[x, z].NeighborRight)
                                instantiateWall(OneNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, 90, 0), parent);
                            else if (TileMap[x, z].NeighborBack)
                                instantiateWall(OneNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, 180, 0), parent);
                            else if (TileMap[x, z].NeighborLeft)
                                instantiateWall(OneNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, -90, 0), parent);
                            break;
                        case 2:
                            if (TileMap[x, z].NeighborForward && TileMap[x, z].NeighborBack)
                                instantiateWall(TwoStraightNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.identity, parent);
                            else if (TileMap[x, z].NeighborRight && TileMap[x, z].NeighborLeft)
                                instantiateWall(TwoStraightNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, 90, 0), parent);
                            else if (TileMap[x, z].NeighborForward )
                            { 
                                if( TileMap[x, z].NeighborRight)
                                    instantiateWall(TwoCornerNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.identity, parent);
                                else if( TileMap[x, z].NeighborLeft)
                                    instantiateWall(TwoCornerNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, -90, 0), parent);
                            }
                            else if (TileMap[x, z].NeighborBack)
                            {
                                if (TileMap[x, z].NeighborRight)
                                    instantiateWall(TwoCornerNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, 90, 0), parent);
                                else if (TileMap[x, z].NeighborLeft)
                                    instantiateWall(TwoCornerNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, 180, 0), parent);
                            }
                            break;
                        case 3:
                            if(!TileMap[x, z].NeighborForward)
                                instantiateWall(ThreeNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.identity, parent);
                            else if (!TileMap[x, z].NeighborRight)
                                instantiateWall(ThreeNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, 90, 0), parent);
                            else if (!TileMap[x, z].NeighborLeft)
                                instantiateWall(ThreeNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, -90, 0), parent);
                            else if (!TileMap[x, z].NeighborBack)
                                instantiateWall(ThreeNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.Euler(0, 180, 0), parent);
                            break;
                        case 4:
                            instantiateWall(FourNeighbors, Vector3.right * x + Vector3.forward * z, Quaternion.identity, parent);
                            break;
                        default:
                            break;
                    }

                }
            }

        foreach(var room in stationSpawner.Rooms)
        {
            Instantiate(FourNeighbors, Vector3.right * (room.x) + Vector3.forward * (room.y + room.h/2 - 1f) + Vector3.up, Quaternion.identity, parent);
            Instantiate(FourNeighbors, Vector3.right * (room.x + room.w/2f -1f) + Vector3.forward * (room.y) + Vector3.up, Quaternion.identity, parent);
            Instantiate(FourNeighbors, Vector3.right * (room.x - room.w / 2f) + Vector3.forward * (room.y) + Vector3.up, Quaternion.identity, parent);
            Instantiate(FourNeighbors, Vector3.right * (room.x) + Vector3.forward * (room.y - room.h / 2f) + Vector3.up, Quaternion.identity, parent);
            Instantiate(FourNeighbors, Vector3.right * (room.x) + Vector3.forward * (room.y ) + Vector3.up, Quaternion.identity, parent);

        }


        done = true;
        Debug.Log("TIme stop at " + (Time.realtimeSinceStartup - time));
    }

    void instantiateWall(GameObject obj, Vector3 position, Quaternion rot, Transform parent)
    {
        for (int i = 0; i < 3; i++)
            Instantiate(obj, position + Vector3.up * i, rot, parent);
    }

    void smooth()
    {
        // smooth
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                int count = 0;
                count += (i > 0 && TileMap[i - 1, j].WallValue == 1) ? 1 : 0;
                count += (j > 0 && TileMap[i, j - 1].WallValue == 1) ? 1 : 0;
                count += (i < size - 1 && TileMap[i + 1, j].WallValue == 1) ? 1 : 0;
                count += (j < size - 1 && TileMap[i, j + 1].WallValue == 1) ? 1 : 0;

                TileMap[i, j].NeighborCount = count;
            }
        var temp = new int[size, size];
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
            {
                temp[i,j] = TileMap[i, j].NeighborCount >= 2 ? 1 : 0;
            }

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                TileMap[i, j].WallValue = temp[i, j];
    }
    bool done = false;
    private void OnDrawGizmos ()
    {
        if (!Application.isPlaying || done)
            return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                if(TileMap[i,j].WallValue == 1)
                Gizmos.DrawCube(new Vector3(j, -1, i), Vector3.one);
    }


    public struct WallObject
    {
        public int WallValue;
        public int NeighborCount;
        public bool NeighborForward;
        public bool NeighborRight;
        public bool NeighborLeft;
        public bool NeighborBack;
    }
}
