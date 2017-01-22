using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreateRoomsTest : MonoBehaviour {

    public List<Room> Rooms = new List<Room>();
    public List<Corridor> Corridors = new List<Corridor>();

    public int RoomMinSize = 4;
    public int RoomMAxSize = 12;
    public int RoomCount = 5;
    public int RoomCountVariation = 2;
    public Vector3 Size;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        if (GUILayout.Button("DO ROOMS"))
            CreateRooms();
        if (GUILayout.Button("Connect Rooms"))
            ConnectRooms();
    }

    void ConnectRooms()
    {
        Rooms.Sort((a, b) => a.Position.y.CompareTo(b.Position.y));
        //Rooms = Rooms.OrderBy((r) => r.Position.y).ThenBy((r) => )
        Corridors = new List<Corridor>();

        //foreach (var r in Rooms)
        for(int i = 0; i < Rooms.Count -1; i++)
        {
            var b1 = new Bounds(Rooms[i].Position, Rooms[i].size);
            var b2 = new Bounds(Rooms[i + 1].Position, Rooms[i + 1].size);

            var p1 = b1.ClosestPoint(Rooms[i + 1].Position);
            var p2 = b2.ClosestPoint(Rooms[i].Position);
            Corridors.Add(new Corridor(p1, p2));
        }
    }

    void CreateRooms()
    {
        var rooms = RoomCount + Random.Range(-RoomCountVariation, RoomCountVariation);
        Rooms = new List<Room>();
        for (int i = 0; i < rooms; i ++)
        {
            Rooms.Add(new Room(new Vector3(getIntRandom( Size.x), getIntRandom(Size.y), getIntRandom(Size.z)), Random.Range(RoomMinSize, RoomMAxSize), Random.Range(RoomMinSize, RoomMAxSize)));
        }
    }

    int getIntRandom(float mod)
    {
        return Mathf.RoundToInt( Random.value * mod);
    }

    void OnDrawGizmos()
    {
        if(Rooms != null)
            Rooms.ForEach((r) => Gizmos.DrawCube(r.Position, r.size + Vector3.up * .5f));
        if (Corridors != null)
            Corridors.ForEach((r) => Gizmos.DrawLine(r.Start, r.End));
    }
    

    [System.Serializable]
    public class Room
    {
        public Vector3 Position;
        public Vector3 size;
        public int Width;
        public int Height;

        public Room(Vector3 p, int w, int h)
        {
            Position = p;
            Width = w;
            Height = h;
            size = Vector3.forward * Height + Vector3.right * Width;
        }
    }

    public class Corridor
    {
        public Vector3 Start;
        public Vector3 End;

        public Corridor(Vector3 s, Vector3 e)
        {
            Start = s;
            End = e;
        }
    }
}
