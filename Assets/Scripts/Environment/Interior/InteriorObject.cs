using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorObject : MonoBehaviour {

    public int SizeX = 1;
    public int SizeY = 1;
    public int SizeZ = 1;
    
    public bool RequiresWall = false;
    public bool TryClusterToSameObject = false;

    public int SpawnValue = 1;
}
