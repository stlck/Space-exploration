using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    /*
     Inventory
     Mission
     (map)
     Health
     CurrentWeapon
    */

    public HealthUI HealthUI;
    public WeaponUI WeaponUI;

    bool initialized = false;

    private void Awake ()
    {
        HealthUI.gameObject.SetActive(false);
        WeaponUI.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (!initialized && !setup())
            return;

    }

    bool setup()
    {
        if (MyAvatar.Instance == null)
            return false;

        initialized = true;

        HealthUI.gameObject.SetActive(true);
        WeaponUI.gameObject.SetActive(true);

        return true;
    }
}

public class GenericComparitor<T> where T: IComparable
{
    public T CurrentValue;
    public Action ComparisonAction;
    public Action<T> UpdateAction;

    public void Compare (T val)
    {
        if (/*val != CurrentValue*/ val.CompareTo(CurrentValue) != 0)
        {
            CurrentValue = val;
            UpdateAction(CurrentValue);
        }
    }
}