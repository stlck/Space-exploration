using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseInterior : MonoBehaviour {

    static List<Vector2Int> crawlDirections = new List<Vector2Int>();
    int currentCrawlIndex = 0;
    Dictionary<InteriorObject, spawnCrawl> spawnNavigator = new Dictionary<InteriorObject, spawnCrawl>();

    public List<InteriorObject> ObjectInRoom;
    protected BspCell TargetRoom;
    protected InstantiatedLocation owningLocation;
    // Use this for initialization
    void Start () {
		
	}

    public void SetupRoom(BspCell room, InstantiatedLocation owner)
    {
        if (crawlDirections.Count == 0)
        {
            crawlDirections.Add(new Vector2Int(1, 0));
            crawlDirections.Add(new Vector2Int(-1, 0));
            crawlDirections.Add(new Vector2Int(0, 1));
            crawlDirections.Add(new Vector2Int(0, -1));

            crawlDirections.Shuffle();
        }
        TargetRoom = room;
        owningLocation = owner;
        SpawnInRoom();
    }

    public virtual void SpawnInRoom(/*BspCell TargetRoom, TileNode[,] RoomTiles*/)
    {

        var count = Random.Range(TargetRoom.CellTiles.Count * .1f, TargetRoom.CellTiles.Count * .3f);
        count = Mathf.Clamp(count, 5, 18);

        for (int i = 0; i < count; )
        {
            var spawn = ObjectInRoom.Where(m => m.SpawnValue < count).GetRandom();
            var tile = TargetRoom.TileNodes.GetRandom();

            spawnObject(spawn, tile);
            i += spawn.SpawnValue;
        }
    }

    protected bool spawnObject(InteriorObject spawn, TileNode tile)
    {
        var canspawn = true;
        //if (spawn.TryClusterToSameObject)
        //{
        //    if (!spawnNavigator.ContainsKey(spawn))
        //        spawnNavigator.Add(spawn, new spawnCrawl() { x = tile.x, y = tile.y, Dir = crawlDirections.GetRandom() });

        //    if (Random.Range(0, 100) > 20)
        //    {
        //        bool foundSpace = false;
        //        int directionsSearched = 0;
        //        while(!foundSpace && directionsSearched < 4)
        //        {
        //            var x = spawnNavigator[spawn].x + spawnNavigator[spawn].Dir.x * spawn.SizeX;
        //            var y = spawnNavigator[spawn].y + spawnNavigator[spawn].Dir.y * spawn.SizeY;
        //            if (x > 0 && x < owningLocation.Size && y > 0 && y < owningLocation.Size && owningLocation.TileMap[x, y].TileValue == 1)
        //            {
        //                tile = owningLocation.TileMap[spawnNavigator[spawn].x + spawnNavigator[spawn].Dir.x * spawn.SizeX, spawnNavigator[spawn].y + spawnNavigator[spawn].Dir.y * spawn.SizeY];
        //                foundSpace = true;
        //            }
        //            else
        //            {
        //                currentCrawlIndex = currentCrawlIndex == 3 ? 0 : currentCrawlIndex++;
        //                spawnNavigator[spawn].Dir = crawlDirections[currentCrawlIndex];
        //            }
        //            directionsSearched++;
        //        }                    
        //    }
        //    else
        //        spawnNavigator[spawn].Dir = crawlDirections.GetRandom();

        //    spawnNavigator[spawn].x = tile.x;
        //    spawnNavigator[spawn].y = tile.y;
        //}

        //if (spawn.RequiresWall && !tile.neighbor2)
        //    canspawn = false;
        //if (spawn.SizeX > 1 || spawn.SizeZ > 1)
        //    for (int x = 0; x < spawn.SizeX; x++)
        //        for (int y = 0; y < spawn.SizeZ; y++)
        //            if (tile.x + x > 0 && tile.x + x < owningLocation.Size && tile.y + y > 0 && tile.y + y < owningLocation.Size && owningLocation.TileMap[tile.x + x, tile.y + y].TileValue != 1)
        //                canspawn = false;

        //if (canspawn)
        //{
        //    var obj = Instantiate(spawn);
        //    obj.transform.SetParent(transform);
        //    obj.transform.position = transform.root.position + new Vector3(tile.x, 1, tile.y);
        //    for (int x = 0; x < spawn.SizeX; x++)
        //        for (int y = 0; y < spawn.SizeZ; y++)
        //            owningLocation.TileMap[tile.x + x, tile.y + y].TileValue = 3;
        //}

        return canspawn;
    }

    class spawnCrawl
    {
        public int x;
        public int y;
        public Vector2Int Dir;
    }
}
