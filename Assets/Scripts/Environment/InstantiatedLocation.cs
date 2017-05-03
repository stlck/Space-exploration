using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatedLocation : MonoBehaviour {

    public Location TargetLocation;
    public Mission Mission;

	// Use this for initialization
	void Start () {
		if(MyAvatar.Instance.isServer)
        {
            switch (TargetLocation.Type)
            {
                case LocationTypes.Asteroid:
                    break;
                case LocationTypes.SpaceEncounter:
                    break;
                case LocationTypes.SpaceStation:
                    break;
            }
        }
	}
	
    void addToLocation()
    {
        
    }

	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 FindOpenSpotInLocation()
    {
        return TargetLocation.GetRandomSpotInLocation() + transform.position;
    }
}
