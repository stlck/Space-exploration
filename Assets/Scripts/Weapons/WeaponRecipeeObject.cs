using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WeaponRecipeeObject : ScriptableObject {

    public int Seed;
    public BaseWeapon OrgWeapon;
    //public Gradient WeaponColors;

    public float MinCooldown;
    public float MaxCooldown;

    public float MinDamage;
    public float MaxDamage;

    public float MinRange;
    public float MaxRange;

  /*  public override string ToString ()
    {
        return Seed + ": " + OrgWeapon.name;
    }*/
}
