﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedLocation : MonoBehaviour {

    public Location TargetLocation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 FindOpenSpotInLocation()
    {
        return TargetLocation.GetRandomSpotInLocation() + transform.position;
    }
}
