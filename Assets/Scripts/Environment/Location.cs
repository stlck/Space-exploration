using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Location : ScriptableObject {

    [ContextMenuItem("TestSpawnLocation", "TestSpawnLocation")]
    public string Name = "";
    public Vector3 Position;
    public int seed = -1;

    public LocationTypes Type;
    public LocationStandings Standing;

    public void TestSpawnLocation()
    {
        var t = GameObject.Find(name);
        if (t != null)
            DestroyImmediate(t);
        var owner = new GameObject(Name);
        owner.transform.position = Position;
        SpawnLocation(owner.transform, seed);
    }

    public virtual void SpawnLocation(Transform owner, int _seed = -1)
    {
        Debug.Log("Location base spawn");
        seed = _seed;
    }
}

public enum LocationTypes
{
    Asteroid,
    SpaceEncounter,
    SpaceStation,
}

public enum LocationStandings
{
    Hostile,
    Friendly,
    Abandoned,
}

public enum TileSet
{
    BrownAsteroid,
    RedAsteroid,
    BlackAsteroid,
    OldStation,
    BlueStation,
}