using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShopObject : MonoBehaviour {

    public List<BaseItem> ForSale = new List<BaseItem>();
    public bool Show = false;

    public Rect WindowPosition = new Rect();
    private Vector2 scrollPos;

    void Awake()
    {
        ForSale = Resources.LoadAll<BaseItem>("").ToList();
    }

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
        GUILayout.Label("old weapons");
        foreach (var w in ForSale)
        {
            GUILayout.BeginHorizontal();
            w.ShopGUI();

            GUILayout.EndHorizontal();
        }
        GUILayout.Label("new weapons");

        //foreach (var item in WeaponValues)
        foreach(var recipee in recipeObjects)
        {
            var item = WeaponValues.First(m => m.Seed == recipee.Seed);

            GUILayout.BeginHorizontal();
            GUILayout.Label(recipee.name);
            GUILayout.Label(item.Damage + "");
            GUILayout.Label(item.Cooldown + "");
            GUILayout.Label(item.Range + "");
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

                foreach (var seed in NetworkHelper.Instance.WeaponSeeds)
                {
                    var recipee = WeaponGenerator.GetRecipee(seed);
                    recipeObjects.Add(recipee);
                    WeaponValues.Add(WeaponGenerator.getBaseWeaponValues(recipee));
                }

                Show = true;
            }
            else
                Show = false;
        }
    }
}
