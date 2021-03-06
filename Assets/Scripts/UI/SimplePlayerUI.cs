﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerUI : MonoBehaviour {

    public Rect WindowRect;
    public Rect InventoryRect;
    private Vector2 scrollPos;
    bool showInventory = false;
    bool showMissions = false;

    // Use this for initialization
    void Start () {
        WindowRect = new Rect(new Vector2(10, Screen.height - 210), new Vector2(100, 200));
        InventoryRect = new Rect(new Vector2(120, Screen.height - 210), new Vector2(300, 200));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUILayout.Window(0, WindowRect, window, "");
        if (showInventory)
            GUILayout.Window(1, InventoryRect, doInventoryWindow, "Inventory");
        if (showMissions)
            GUILayout.Window(1, InventoryRect, doMissionWindow, "Mission");
    }

    void window(int id)
    {
        //GUILayout.Label("Health");
        //GUILayout.HorizontalSlider(MyAvatar.Instance.MyStats.CurrentHealth / MyAvatar.Instance.MyStats.MaxHealth, 0f , 1f);

        GUILayout.Label("CREDITS");
        GUILayout.TextField("$: " + TeamStats.Instance.Credits);

        if(GUILayout.Button("Inventory"))
        {
            // weapons
            showInventory = !showInventory;
            if (showInventory && showMissions)
                showMissions = false;
        }
        if(GUILayout.Button("Missions"))
        {
            showMissions = !showMissions;
            if (showMissions && showInventory)
                showInventory = false;
        }
    }

    void doInventoryWindow(int id)
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
        /*foreach(var i in MyAvatar.Instance.InventoryItems)
        {
            GUILayout.BeginHorizontal();
            i.InventoryGUI();
            GUILayout.EndHorizontal();
        }*/
        foreach(var e in MyAvatar.Instance.AvatarWeaponHandler.InstantiatedWeapons)
        {
            GUILayout.BeginHorizontal();
            e.Weapon.InventoryGUI();
            GUILayout.EndHorizontal();
        }
        foreach (var e in MyAvatar.Instance.InventoryItems)
        {
            GUILayout.BeginHorizontal();
            e.InventoryGUI();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    void doMissionWindow(int id)
    {
        foreach (var m in NetworkHelper.Instance.Missions)
            GUILayout.Label((LocationTypes) m.LocationType + ": " + m.Name + " at " + m.Location.Position);
    }
}
