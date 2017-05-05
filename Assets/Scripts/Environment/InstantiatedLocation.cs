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
            var percentage = Random.Range(0, 100);
            if (percentage > 80)
            {
                    // spawn some simple Interior
                var positions = GetRoomPositions(r, percentage);
                for (int i = 0; i < positions.Count; i++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.SetParent(transform);
                    cube.transform.localPosition = new Vector3(r.x + positions[i].x, 1, r.y + positions[i].z); // + //Random.Range(-.5f, .5f) * r.w * Vector3.right + Random.Range(-.5f, .5f) * r.h * Vector3.forward;
                }

            }
        }

        if(station.Standing == LocationStandings.Hostile)
        {

        }
    }

    public Vector3 FindOpenSpotInLocation()
    {
        return TargetLocation.GetRandomSpotInLocation() + transform.position;
    }

    public List<Vector3> GetRoomPositions (BspCell r, float percentage)
    {
        var ret = new List<Vector3>();
        var p = percentage - .8f;
        p *= 5f;
        var amount = Random.Range(2, 6);

        if (p < .333f)
        {
            var offset = Random.Range(-1f, 1);
            for (int i = 0; i < amount; i++)
                ret.Add(r.h / 2 * Vector3.forward * (i - amount / 2) + r.w * offset * Vector3.right);
        }
        else if (p < .666f)
        {
            var offset = Random.Range(-1f, 1);
            for (int i = 0; i < amount; i++)
                ret.Add(r.w / 2 * Vector3.right * (i - amount / 2) + r.h * offset * Vector3.forward);
        }
        else
        {
            for (int i = 0; i < amount; i++)
                ret.Add(Random.Range(-.5f, .5f) * r.w * Vector3.right + Random.Range(-.5f, .5f) * r.h * Vector3.forward);
        }

        return ret;
    }
}
