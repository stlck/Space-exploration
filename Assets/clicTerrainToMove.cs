using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clicTerrainToMove : MonoBehaviour {

    public CreateBoxMan Target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            var r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            if(Physics.Raycast(r, out hitinfo,300))
            {
                Target.stateTarget = hitinfo.point;
                Target.moveTo();
            }
        }
	}
}
