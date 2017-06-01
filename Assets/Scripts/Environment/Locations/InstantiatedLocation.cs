using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InstantiatedLocation : MonoBehaviour {

    public Location TargetLocation;
    public Mission Mission;
    //public TileNode[,] TileMap;
    //public TileSet TileSet;
    //public int Size;
    //public static List<BaseInterior> InteriorPrefabs = new List<BaseInterior>();
    //List<Vector2Int> spawnLocations = new List<Vector2Int>();

    //private void Awake()
    //{
    //    //if (InteriorPrefabs.Count == 0)
    //    //{
    //    //    InteriorPrefabs = Resources.LoadAll<BaseInterior>("BaseInteriors").ToList();
    //    //}
    //}

    // Use this for initialization
    void Start () {
        switch (TargetLocation.Type)
        {
            case LocationTypes.Asteroid:
                break;
            case LocationTypes.SpaceEncounter:
                break;
            case LocationTypes.SpaceStation:
                addToLocationStation();
                break;
        }
    }

    //public virtual void BlockHit(Vector3 position)
    public virtual void BlockHit(int x, int y, int z)
    {

    }

    void addToLocationStation()
    {
        //var station = (LocationStation)TargetLocation;
        //Random.InitState(station.seed);

        //foreach (var r in station.Rooms)
        //{
        //    var percentage = Random.Range(0, 100);
        //    if (percentage > 20)
        //    {
        //        //var roomSpawn = new GameObject().AddComponent<BaseInterior>();
        //        var roomSpawn = Instantiate(InteriorPrefabs.GetRandom(), transform);
        //        roomSpawn.transform.SetParent(transform);
        //        roomSpawn.SetupRoom(r, this);
        //    }
        //}

        //if(station.Standing == LocationStandings.Hostile)
        //{
            
        //}
        //for(int i = 0; i < Size; i++)
        //    for(int j = 0; j < Size; j++)
        //        if(TileMap[i,j].TileValue == 1)
        //            spawnLocations.Add(new Vector2Int(i,j));

                //tileMeshIt();
    }

    //void tileMeshIt()
    //{
    //    var set = Resources.LoadAll<LocationTileSet>("TileSets/" + TileSet.ToString())[0];
    //    for (int i = 0; i < Size; i++)
    //        for (int j = 0; j < Size; j++)
    //        {
    //            var tn = TileMap[i, j];
    //            if (tn.TileValue == 0)
    //            {
    //                // outside walls
    //                if (tn.neighbor2)
    //                {
    //                    for (int y = 0; y < 5; y++)
    //                        CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.up * y, Quaternion.identity, transform);
    //                }
    //            }
    //            // inside with floor
    //            else if (tn.TileValue == 1 || tn.TileValue == 3)
    //            {
    //                // ground
    //                CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.Ground], Vector3.right * i + Vector3.forward * j, Quaternion.identity, transform);
    //                // layer beneath ground
    //                CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.down, Quaternion.identity, transform);
    //                // roof
    //                CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.InnerWall], Vector3.right * i + Vector3.forward * j + Vector3.up * 5, Quaternion.identity, transform, 9);

    //            }
    //            // inside walls
    //            else if (tn.TileValue == 2)
    //            {
    //                // layer beneath ground
    //                CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.OuterWall], Vector3.right * i + Vector3.forward * j + Vector3.down, Quaternion.identity, transform);
    //                // roof
    //                CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.InnerWall], Vector3.right * i + Vector3.forward * j + Vector3.up * 5, Quaternion.identity, transform, 9);
    //                // inside wall
    //                for (int y = 0; y < 5; y++)
    //                    CoroutineSpawner.Instance.Enqueue(set.GroundTiles[LocationTileSet.Room], Vector3.right * i + Vector3.forward * j + Vector3.up * y, Quaternion.identity, transform);
    //            }
    //        }
    //}

    public Vector3 FindOpenSpotInLocation()
    {
        //var p = spawnLocations.GetRandom();
        //return transform.position + new Vector3(p.x, 0, p.y);
        Debug.LogWarning("Get spawn position");
        return Vector3.zero;
    }
}
