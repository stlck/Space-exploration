using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestSearch : MonoBehaviour {

    public LocationStation Station;
    public WalkingEnemy e;
    public BestFirstSearch bfs;
    int[,] map;
    public Transform MoveTarget;
    public Transform MoveToTarget;
    Vector3 prev;
    bool moving = false;
    float timer = 0f;
    public LineRenderer line;
    public float speed = 2f;

    public List<Vector3> route = new List<Vector3>();

    void Awake()
    {
        map = new StationSpawner().Generate(transform, Station.seed, Station.Size, Station.Splits, Station.MinRoomSize, Station.HalfCorridorSize, Station.TileSet, false);
        
    }
    // Use this for initialization
    void Start () {

        bfs = new BestFirstSearch(Station.Size, Station.Size);
        var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
        c.GetComponent<Collider>().enabled = false;

        for (int i = 0; i < Station.Size; i++)
            for (int j = 0; j < Station.Size; j++)
            {
                if (map[i, j] == 1)
                    Instantiate(c, Vector3.right * i + Vector3.forward * j, Quaternion.identity, transform);
                else
                    bfs.AddObstacle(i, j);
            }
        
    }
	
	// Update is called once per frame
	void Update () {
        
		if(route.Any())
        {
            timer += Time.deltaTime * speed;
            MoveTarget.position = Vector3.Lerp(prev, route[0] + Vector3.up, timer);
            if(timer >= 1f)
            {
                route.RemoveAt(0);
                for (int i = 0; i < route.Count; i++)
                    line.SetPosition(i, route[i] + Vector3.up);
                prev = MoveTarget.position;
                timer = 0f;
            }
        }

        else
        {
            if(Vector3.Distance(MoveTarget.position, MoveToTarget.position)< 5)
            { 
                bool foundPos = false;
                while (!foundPos)
                {
                    var x = Random.Range(0, Station.Size);
                    var y = Random.Range(0, Station.Size);
                    if(map[x,y] == 1)
                    {
                        MoveToTarget.transform.position = new Vector3(x, 1, y);
                        foundPos = true;
                    }
                }
            }
            DoRoute();
        }
    }

    void OnGUI()
    {
        if (GUILayout.Button("route"))
            DoRoute();
    }

    void OnDrawGizmos()
    {
        //foreach(var r in route)
        Gizmos.color = Color.red;
        for (int i = 0; i < route.Count - 1; i++)
            Gizmos.DrawLine(route[i] + Vector3.up, route[i + 1] + Vector3.up);
    }

    public void DoRoute()
    {
        var x = (int)MoveTarget.transform.position.x;
        var z = (int)MoveTarget.transform.position.z;
        var tox = (int)MoveToTarget.position.x;
        var toz = (int)MoveToTarget.position.z;

        Debug.Log(x + "," + z + " to " + tox + "," + toz);
        //route.Clear();
        route = bfs.FindPath(x, z, tox, toz);
        prev = MoveTarget.position;

        line.numPositions = route.Count;
        for(int i = 0; i < route.Count ; i++)
        line.SetPosition(i, route[i] + Vector3.up);
        //e.Route = route;
        //e.SetChaseState(transform);
    }
}
