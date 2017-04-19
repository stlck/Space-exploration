using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WarpShip : NetworkBehaviour, IShipSpawnObject {

    public Rect MapRect;
    bool showMap = false;
    Ship owningShip;

    // Use this for initialization
    void Start () {
        MapRect = new Rect(new Vector2(120, Screen.height - 210), new Vector2(200, 200));
        owningShip = GetComponentInParent<Ship>();
    }
    
    void OnGUI()
    {
        if (showMap)
            GUILayout.Window(3, MapRect, map, "MAP");
    }

    void map(int id)
    {
        if (GUILayout.Button("close"))
            showMap = false;

        foreach (var l in NetworkHelper.Instance.Missions)
        {
            if (GUILayout.Button(l.Location.Name))
            {
                NetworkHelper.Instance.RpcSpawnLocation(l.name, l.Seed);
                owningShip.WarpTo(l.Location.Position - Vector3.right * 10);
            }
            //MyAvatar.Instance.CmdWarpShip(owningShip, l.Position);
        }
    }

    public void OnMouseUp()
    {
        showMap = !showMap;
    }

    public List<Vector2Int> TileConfig()
    {
        // not needed,
        return new List<Vector2Int>();
    }

    public void SetTilePosition(Vector2Int position)
    {
        // not needed,
    }
}
