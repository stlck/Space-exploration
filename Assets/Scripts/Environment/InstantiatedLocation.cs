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
                    addToLocationStation();
                    break;
            }
        }
	}
	
    void addToLocationStation()
    {
        var station = (LocationStation)TargetLocation;
        Random.InitState(station.seed);

        foreach(var r in station.Rooms)
        {
            if(Random.Range(0,100) > 75)
            {
                // spawn some simple Interior
                //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.SetParent(transform);
                //cube.transform.localPosition = new Vector3(r.x, 1, r.y) + Random.Range(-.5f, .5f) * r.w * Vector3.right + Random.Range(-.5f, .5f) * r.h * Vector3.forward;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 FindOpenSpotInLocation()
    {
        return TargetLocation.GetRandomSpotInLocation() + transform.position;
    }
}
