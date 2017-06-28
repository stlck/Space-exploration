using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarScan : MonoBehaviour {

    public GridGraph StationGraph;

	// Use this for initialization
	void Start () {
        var station = Resources.Load<Location>("Locations\\StartStation");
        station.SpawnLocation(transform);
        StationGraph = AstarPath.active.data.AddGraph(typeof(GridGraph)) as GridGraph;
        
        StationGraph.neighbours = NumNeighbours.Eight;
        StationGraph.SetDimensions(100, 100, 1);
        StationGraph.center = new Vector3(10, -1, 10);
        StationGraph.maxClimb = 1;
        StationGraph.maxSlope = 0;
        StationGraph.collision.heightCheck = true;
        StationGraph.collision.height = 1;
        StationGraph.collision.diameter = 2;
        StationGraph.collision.thickRaycast = true;
        StationGraph.collision.thickRaycastDiameter = 2f;
        StationGraph.collision.fromHeight = 5f;
        StationGraph.collision.unwalkableWhenNoGround = false;

        StartCoroutine(waitAndScan());
	}

    IEnumerator waitAndScan()
    {
        yield return new WaitForSeconds(.5f);
        AstarPath.active.Scan();
    }
}
