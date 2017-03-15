using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateFragment : BaseAddForceObject
{
    public MeshRenderer renderer;

    public void SetMaterial(Material t)
    {
        renderer.sharedMaterial = t;
    }
    
}
