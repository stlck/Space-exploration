using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour {

    public static List<WeaponRecipeeObject> WeaponRecipees;
    public static List<BaseWeapon> LoadedWeapons = new List<BaseWeapon>();
    public BaseWeapon.BaseWeaponValues test;
    public WeaponRecipeeObject testrecipee;

    // Use this for initialization
    void Start () {
        WeaponRecipees = Resources.LoadAll<WeaponRecipeeObject>("WeaponRecipees").ToList();
        testrecipee = GetRecipee(Random.Range(0, 10000));
        test = getBaseWeaponValues(testrecipee);
	}

    void OnGUI()
    {
        /* for tests */
        if (GUILayout.Button("New"))
        {
            testrecipee = GetRecipee(Random.Range(0, 10000));
            test = getBaseWeaponValues(testrecipee);
        }

        GUILayout.TextField(testrecipee.ToString());

        GUI.color = test.WeaponColor;
        GUILayout.TextField("Damage : " + test.Damage);
        GUILayout.TextField("Cooldown : " + test.Cooldown);
        GUILayout.TextField("Range : " + test.Range);
    }

    public static WeaponRecipeeObject GetRecipee(int seed)
    {
        if (WeaponRecipees == null)
            WeaponRecipees = Resources.LoadAll<WeaponRecipeeObject>("WeaponRecipees").ToList();

        Random.InitState(seed);
        
        var ret = WeaponRecipees.GetRandom();
        ret.Seed = seed;
        return ret;
    }

    public static BaseWeapon.BaseWeaponValues getBaseWeaponValues(WeaponRecipeeObject recipee)
    {
        Random.InitState(recipee.Seed);

        var ret = new BaseWeapon.BaseWeaponValues();
        ret.Seed = recipee.Seed;

        ret.Cooldown = Random.Range(recipee.MinCooldown, recipee.MaxCooldown);
        ret.WeaponColor = new Color(Random.value,Random.value,Random.value,1);
        //ret.WeaponColor = recipee.WeaponColors.Evaluate(Random.value);
        ret.Damage = Random.Range(recipee.MinDamage, recipee.MaxDamage);
        ret.Range = Random.Range(recipee.MinRange, recipee.MaxRange);

        Debug.Log("recipee seed " + ret.Seed + ". CD: " + ret.Cooldown);

        return ret;
    }

    public static BaseWeapon InstantiateWeapon (int id, Transform parent)
    {
        if(LoadedWeapons == null || LoadedWeapons.Count == 0)
            LoadedWeapons = Resources.LoadAll<BaseWeapon>("Weapons").ToList();

        BaseWeapon w = Instantiate(LoadedWeapons.First(m => m.Id == id));

        w.transform.parent = parent;
        w.transform.localPosition = w.transform.position;
        w.transform.localEulerAngles = Vector3.zero;
        
        return w;
    }
    public static BaseWeapon InstantiateWeapon (BaseWeapon weapon, Transform parent)
    {
        BaseWeapon w = Instantiate(weapon);

        w.transform.parent = parent;
        w.transform.localPosition = w.transform.position;
        w.transform.localEulerAngles = Vector3.zero;

        return w;
    }

    [System.Serializable]
    public struct InstantiatedWeapon
    {
        public int Id;
        public int Seed;
        public BaseWeapon Weapon;
        public GameObject GameObject;
    }
}

/*[System.Serializable]
public class WeaponRecipee
{
    public int Seed;
    public BaseWeapon OrgWeapon;
    //public Gradient WeaponColors;

    public float MinCooldown;
    public float MaxCooldown;

    public float MinDamage;
    public float MaxDamage;

    public float MinRange;
    public float MaxRange;

    public override string ToString ()
    {
        return Seed + ": " + OrgWeapon.name; ;
    }
}*/


/*
All:
 - Color Event
 - Damage
 - Cooldown

projectiles:
 - projectile

stream:
 - range
 - width

energy:
 - range
 - width
 - Hit effect
*/
