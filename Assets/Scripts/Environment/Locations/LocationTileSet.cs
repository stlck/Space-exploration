using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LocationTileSet : ScriptableObject {

    public static int GetRandom()
    {
        return UnityEngine.Random.Range(0, 3);
    }

    public static int Ground = 0;
    public static int InnerWall = 1;
    public static int OuterWall = 2;
    public static int Room = 3;
    
    [Header("ground 0, innerwall, outerwall, room")]
    public List<Transform> GroundTiles;
    public List<Material> Materials;
}
