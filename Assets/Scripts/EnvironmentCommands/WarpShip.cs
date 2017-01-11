using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpShip : MonoBehaviour {

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
        GUILayout.Label("Host only");

        if (MyAvatar.Instance.Identity.isServer)
        foreach(var l in MyAvatar.Instance.EnvironmentCreator.MyLocations)
        {
            if (GUILayout.Button(l.Name))
                warpTo(l);
        }
    }

    void warpTo(Location l)
    {
        owningShip.Warping = true;
        owningShip.WarpPosition = l.Position;
    }

    public void OnMouseUp()
    {
        showMap = !showMap;
    }
}
