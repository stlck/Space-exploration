using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StorageRoom : BaseInterior {

    public InteriorObject WallObject;


    public override void SpawnInRoom()
    {

        //var minX = TargetRoom.TileNodes.Min(m => m.x)+1;
        //var maxX = TargetRoom.TileNodes.Max(m => m.x)-1;
        //var minY = TargetRoom.TileNodes.Min(m => m.y);
        //var maxY = TargetRoom.TileNodes.Max(m => m.y)-1;

        //for(int i = minX; i < maxX; i++)
        //    if(owningLocation.TileMap[i, minY].TileValue == 1 && owningLocation.TileMap[i, minY].neighbor2)
        //        spawnObject(WallObject, owningLocation.TileMap[i, minY]);
        //    else if (owningLocation.TileMap[i, maxY].TileValue == 1 && owningLocation.TileMap[i, maxY].neighbor2)
        //        spawnObject(WallObject, owningLocation.TileMap[i, maxY]);

        //for (int i = minY; i < maxY; i++)
        //    if (owningLocation.TileMap[i, minX].TileValue == 1 && owningLocation.TileMap[i, minX].neighbor2)
        //        spawnObject(WallObject, owningLocation.TileMap[i, minX]);
        //    else if (owningLocation.TileMap[i, maxX].TileValue == 1 && owningLocation.TileMap[i, maxX].neighbor2)
        //        spawnObject(WallObject, owningLocation.TileMap[i, maxX]);

        //base.SpawnInRoom();

    }
}
