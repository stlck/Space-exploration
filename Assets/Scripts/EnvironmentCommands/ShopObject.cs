using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShopObject : MonoBehaviour {

    public bool Show = false;
    public Rect WindowPosition = new Rect();
    private Vector2 scrollPos;

    // Use this for initialization
    void Start () {
	    WindowPosition = new Rect(new Vector2(200, 200), new Vector2(700, 500));
    }
	
	// Update is called once per frame
	void Update () {
        if (Show && !canShow() || Input.GetKeyDown(KeyCode.Escape))
            Show = false;
	}

    bool canShow()
    {
        return Vector3.Distance(MyAvatar.Instance.transform.position, transform.position) < 3;
    }

    void OnGUI()
    {
        if(Show)
        {
            WindowPosition = GUILayout.Window(2, WindowPosition, ShopWindow, "BUY");
        }
    }

    void ShopWindow(int id)
    {
        if (GUILayout.Button("X"))
            Show = false;

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);

        GUILayout.Label("WEAPONS");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Purchase", GUILayout.Width(100));
        GUILayout.Label("Name", GUILayout.Width(140));
        GUILayout.Label("Damage", GUILayout.Width(100));
        GUILayout.Label("Cooldown", GUILayout.Width(100));
        GUILayout.Label("Range", GUILayout.Width(100));
        GUILayout.EndHorizontal();

        for (int i = 0; i < recipeObjects.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("$" + recipeObjects[i].OrgWeapon.Price, GUILayout.Width(100)) && recipeObjects[i].OrgWeapon.CanBuy())
                recipeObjects[i].OrgWeapon.BuyWeapon(recipeObjects[i].Seed);
            GUILayout.Label(recipeObjects[i].name, GUILayout.Width(140));
            GUILayout.Label(WeaponValues[i].Damage + "", GUILayout.Width(100));
            GUILayout.Label(WeaponValues[i].Cooldown + "", GUILayout.Width(100));
            GUILayout.Label(WeaponValues[i].Range + "", GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    List<WeaponRecipeeObject> recipeObjects;
    List<BaseWeapon.BaseWeaponValues> WeaponValues;

    void OnMouseUp()
    {
        if(!canShow())
            RangeIndicator.Instance.TurnOn(transform.position, 3);
        else
        {
            if (!Show )
            {
                recipeObjects = new List<WeaponRecipeeObject>();
                WeaponValues = new List<BaseWeapon.BaseWeaponValues>();

                for(int i = 0; i < NetworkHelper.Instance.WeaponSeeds.Count; i++)
                {
                    var recipee = WeaponGenerator.GetRecipee(NetworkHelper.Instance.WeaponSeeds[i]);
                    recipeObjects.Add(recipee);
                    var values = WeaponGenerator.getBaseWeaponValues(recipee);
                    WeaponValues.Add(values);
                }

                Show = true;
            }
            else
                Show = false;
        }
    }
}
