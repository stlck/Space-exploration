using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelCollider : MonoBehaviour {

    public int VoxelX { get; set; }
    public int VoxelY { get; set; }
    public int VoxelZ { get; set; }

    public InstantiatedLocation owner;

    private void OnDestroy()
    {
        if(Application.isPlaying)
            owner.BlockHit(VoxelX,VoxelY, VoxelZ);
        //Destroy(gameObject);
    }
}
