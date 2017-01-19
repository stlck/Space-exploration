using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneratorTest : MonoBehaviour {

    [ContextMenuItem("newGen2", "newGen2")]
    public LocationStation Test;

    MeshGenerator meshGen;

    // Use this for initialization
    void Start () {
        meshGen = new MeshGenerator();

    }
	
	// Update is called once per frame
	public void newGen2 () {
        if(meshGen == null)
            meshGen = new MeshGenerator();

        meshGen.Test = Test;
        meshGen.newGen2();
        GetComponent<MeshFilter>().mesh = meshGen.output;
    }
}
