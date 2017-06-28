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

    //public BestFirstSearch BestFirstSearch;

    /// <summary>
    /// Test only
    /// </summary>
    public void TestSpawnLocation()
    {
        var t = GameObject.Find(name);
        if (t != null)
            DestroyImmediate(t);
        var owner = new GameObject(Name);
        owner.transform.position = Position;
        SpawnLocation(owner.transform,  seed);

    }

    /// <summary>
    /// creates the mesh or tile map
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="_seed"></param>
    /// <returns></returns>
    public virtual InstantiatedLocation SpawnLocation (Transform owner, int _seed = -1)
    {
        seed = _seed;

        if (seed >= 0)
            UnityEngine.Random.InitState(seed);

        var go = new GameObject().AddComponent<InstantiatedLocation>();
        go.transform.position = Position;
        go.TargetLocation = this;
        go.name = this.GetType() + " " + seed;

        return go;
    }

    /// <summary>
    /// Test only
    /// </summary>
    public virtual void ShowCreator()
    {
        if (GUILayout.Button("new Position : " + Position))
            Position = UnityEngine.Random.Range(-1f,1f) * 500 * Vector3.right + UnityEngine.Random.Range(-1f, 1f) * 500 * Vector3.forward;

        if (GUILayout.Button("new Seed : " + seed))
            seed = UnityEngine.Random.Range(0, 20000);

        Type = (LocationTypes)GUILayout.SelectionGrid((int)Type, System.Enum.GetNames(typeof(LocationTypes)), 3);
        Standing = (LocationStandings)GUILayout.SelectionGrid((int)Standing, System.Enum.GetNames(typeof(LocationStandings)), 3);
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