using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Location : ScriptableObject {

    public string Name = "";
    public Vector3 Position;

    public LocationTypes Type;
    public LocationStandings Standing;

    public void TestSpawnLocation()
    {
        var t = GameObject.Find(name);
        if (t != null)
            Destroy(t);
        var owner = new GameObject(Name);
        owner.transform.position = Position;
        SpawnLocation(owner.transform);
    }

    public virtual void SpawnLocation(Transform owner)
    {

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
}