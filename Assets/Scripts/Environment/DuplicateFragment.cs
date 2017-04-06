using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateFragment : MonoBehaviour
{
    public MeshRenderer renderer;
    public ParticleSystemRenderer ParticleSystemRenderer;

    public void SetMaterial(Material t)
    {
        if(renderer != null)
            renderer.sharedMaterial = t;
        if (ParticleSystemRenderer != null)
            ParticleSystemRenderer.sharedMaterial = t;
        
    }
    
}
