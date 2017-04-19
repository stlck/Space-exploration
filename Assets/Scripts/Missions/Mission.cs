using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour {

    public string Name { get; set; }
    public int Seed { get; set; }
    public int LocationType { get; set; }
    public Location Location;
    public int Level { get; set; }

    public Location InstantiateMission ()
    {

        switch(LocationType)
        {
            
            case 1:
                Location = new LocationAsteroid();
                break;
            case 0:
            default:
                Location = new LocationStation();
                break;
        }

        Location.seed = Seed;
        Location.Position = Vector3.right * Random.Range(-1000, 1000) + Vector3.forward * Random.Range(-1000, 1000);
        Location.Name = Name;

        return Location;
    }
}
