using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelCollider : MonoBehaviour {

    public int VoxelX { get; set; }
    public int VoxelY { get; set; }
    public int VoxelZ { get; set; }

    public BspStationTest owner;

    private void OnMouseDown()
    {
        owner.ColliderHit(VoxelX,VoxelY, VoxelZ);
        Destroy(gameObject);
    }
}
