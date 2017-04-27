using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour {

    public List<WeaponRecipee> WeaponRecipees;
    public BaseWeapon.BaseWeaponValues test;
    public WeaponRecipee testrecipee;

    // Use this for initialization
    void Start () {
        testrecipee = GetRecipee(Random.Range(0, 10000));
        test = getBaseWeaponValues(testrecipee);
	}

    void OnGUI()
    {
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

    public WeaponRecipee GetRecipee(int seed)
    {
        Random.InitState(seed);
        var ret = WeaponRecipees.GetRandom();
        ret.Seed = seed;
        return ret;
    }

    public BaseWeapon.BaseWeaponValues getBaseWeaponValues( WeaponRecipee recipee)
    {
        Random.InitState(recipee.Seed);

        var ret = new BaseWeapon.BaseWeaponValues();
        ret.Seed = recipee.Seed;

        ret.Cooldown = Random.Range(recipee.MinCooldown, recipee.MaxCooldown);
        ret.WeaponColor = new Color(Random.value,Random.value,Random.value,1);
        //ret.WeaponColor = recipee.WeaponColors.Evaluate(Random.value);
        ret.Damage = Random.Range(recipee.MinDamage, recipee.MaxDamage);
        ret.Range = Random.Range(recipee.MinRange, recipee.MaxRange);

        return ret;
    }

}

[System.Serializable]
public class WeaponRecipee
{
    public int Seed;
    public BaseWeapon OrgWeapon;
    public Gradient WeaponColors;

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
}


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
