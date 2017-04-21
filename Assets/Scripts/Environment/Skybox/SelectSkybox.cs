using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectSkybox : MonoBehaviour {

    //public Cubemap map;
    [ContextMenuItem("test","test")]
    public Material target;
    public List<Cubemap> Textures = new List<Cubemap>();
    
    // Use this for initialization
    void Start ()
    {
        test();
        //target.mainTexture = Textures[Random.Range(0, Textures.Count)];
        //target.texture = Textures[Random.Range(0, Textures.Count)];
    }

    public void test()
    {
        var tTexture = Textures[Random.Range(0, Textures.Count)];
        target.SetFloat("_Rotation", Random.Range(0, 360));
        target.SetTexture("_Tex", tTexture);
    }
}
