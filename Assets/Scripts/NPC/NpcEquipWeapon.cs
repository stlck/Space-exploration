using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NpcEquipWeapon : NetworkBehaviour {

    public NpcBase WeaponTarget;

    private void Awake ()
    {
        if (WeaponTarget == null)
        {
            WeaponTarget = GetComponent<NpcBase>();
            if (WeaponTarget == null)
                this.enabled = false;
        }
    }
    
	// Use this for initialization
	void Start () {
		if(isServer && WeaponTarget != null && WeaponTarget.Weapon == null)
        {
            RpcNpcEquipRandomWeapon();

            WeaponTarget.Weapon.WeaponValues.Damage *= .33f * (WeaponTarget.Level);
        }
	}

    [ClientRpc]
    public void RpcNpcEquipWeapon (int seed)
    {
        var recipee = WeaponGenerator.GetRecipee(seed);
        var values = WeaponGenerator.getBaseWeaponValues(recipee);
        WeaponTarget.Weapon = WeaponGenerator.InstantiateWeapon(recipee.OrgWeapon.Id, WeaponTarget.WeaponPoint ?? transform);
        WeaponTarget.Weapon.WeaponValues = values;
    }

    [ClientRpc]
    public void RpcNpcEquipRandomWeapon ()
    {
        var recipee = WeaponGenerator.GetRecipee(UnityEngine.Random.Range(0, 40000));
        var values = WeaponGenerator.getBaseWeaponValues(recipee);
        WeaponTarget.Weapon = WeaponGenerator.InstantiateWeapon(recipee.OrgWeapon.Id, WeaponTarget.WeaponPoint ?? transform);
        WeaponTarget.Weapon.WeaponValues = values;
    }
}
