using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateFragment : MonoBehaviour
{
    public MeshRenderer renderer;
    public ParticleSystemRenderer ParticleSystemRenderer;
    ParticleSystem.MainModule main;
    public ParticleSystem particles;

    public void SetMaterial(Material t)
    {
        if(renderer != null)
            renderer.sharedMaterial = t;
        if (ParticleSystemRenderer != null)
            ParticleSystemRenderer.material = t;
    }
    
    public void SetColor(Color BlockColor)
    {
        main = particles.main;
        //if (main != null)
        main.startColor = BlockColor;
    }
}
