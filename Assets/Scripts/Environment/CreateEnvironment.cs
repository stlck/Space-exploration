﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CreateEnvironment : MonoBehaviour {

    public ProceduralBlocks BlockGenerator;
    public List<Transform> GroundTiles;

    public List<Location> MyLocations = new List<Location>();
    public List<Location> SpawnedLocations = new List<Location>();

    // Use this for initialization
    void Start () {
        MyLocations = Resources.LoadAll<Location>("").ToList();
    }

    void Update()
    {
        // spawn terrain in vicinity (local only)
        var close = MyLocations.Where(m => Vector3.Distance(transform.position, m.Position) < 100);
        if (close.Any(m => !SpawnedLocations.Contains(m)))
        {
            // spawn 
            foreach (var c in close)
            {
                if (c.Type == LocationTypes.Asteroid || c.Type == LocationTypes.SpaceStation)
                {
                    var go = new GameObject(c.name);
                    go.transform.position = c.Position;
                    CreateLevel(c, go.transform);
                    SpawnedLocations.Add(c);
                }
                //else if (c.Type == LocationTypes.SpaceEncounter && Identity.isServer)
                //{
                //    // spawn ships
                //}
            }
        }
    }

    public void CreateLevel(Location l, Transform owner = null)
    {
        var s = (Vector3.forward + Vector3.right) * l.TileSize + Vector3.up;
        var tiles = l.MapTiles;// BlockGenerator.GenerateMap(Width, Height);

        for (int i = 0; i < l.Size; i++)
            for (int j = 0; j < l.Size; j++)
            {
                var tile = tiles[i, j];

                if (GroundTiles[tile] != null)
                {
                    var p = l.Position + Vector3.right * i * l.TileSize + Vector3.forward * j * l.TileSize + Vector3.down / 2;

                    if (tile == 3)
                    {
                        for(int k = 0; k < 4; k++)
                        {
                            Instantiate(GroundTiles[tile], p + Vector3.up * k, Quaternion.identity, owner ?? transform);
                        }
                        tile = 0;
                    }

                    var t = Instantiate(
                        GroundTiles[tile],
                        p, 
                        Quaternion.identity) as Transform;

                    t.localScale = s;
                    t.SetParent(owner ?? transform);
                }
            }
    }

    public void DestroyLevel()
    {
        Destroy(gameObject);
    }
}
