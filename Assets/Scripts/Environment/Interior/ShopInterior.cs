using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInterior : BaseInterior {

    public List<InteriorObject> ShopObjects = new List<InteriorObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SpawnInRoom()
    {
        //foreach(var so in ShopObjects)
        //{
        //    var tile = owningLocation.TileMap[TargetRoom.x, TargetRoom.y];
        //    //var tile = TargetRoom.TileNodes.GetRandom();
        //    spawnObject(so, tile);
        //}

        base.SpawnInRoom();
    }
}
