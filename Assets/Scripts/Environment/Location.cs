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

    public BestFirstSearch BestFirstSearch;

    public void TestSpawnLocation()
    {
        var t = GameObject.Find(name);
        if (t != null)
            DestroyImmediate(t);
        var owner = new GameObject(Name);
        owner.transform.position = Position;
        SpawnLocation(owner.transform, seed);

    }

    public virtual InstantiatedLocation SpawnLocation (Transform owner, int _seed = -1)
    {
        Debug.Log("Location base spawn");
        seed = _seed;

        var go = new GameObject().AddComponent<InstantiatedLocation>();
        go.TargetLocation = this;

        return go;
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

[System.Serializable]
public class Vector2Int
{
    public int x;
    public int y;

    public Vector2Int(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    
}

//public static class LocationTileSet
//{
//    public static int Ground = 0;
//    public static int InnerWall = 1;
//    public static int OuterWall = 2;
//    public static int Room = 3;

//    public static int GetRandom()
//    {
//        return UnityEngine.Random.Range(0, 3);
//    }
//}