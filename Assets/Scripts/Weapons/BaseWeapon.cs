using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class BaseWeapon : BaseItem
{
    public float DamagePerHit = 1;
    public int Id = -1;

    public float Cooldown = .2f;
    public float CurrentCooldown = 0f;
    public Color WeaponColor = Color.green;

    public BaseWeaponValues WeaponValues;

    // Use this for initialization
    public virtual void Start () {
		
	}

    // Update is called once per frame
    public virtual void Update () {
        CurrentCooldown += Time.deltaTime;
	}

    public virtual bool CanFire()
    {
        return CurrentCooldown >= Cooldown;
    }

    public virtual void FireWeapon()
    {
        CurrentCooldown = 0f;
    }

    public override bool CanBuy()
    {
        return base.CanBuy();
    }

    public override void BuyItem()
    {
        base.BuyItem();

        MyAvatar.Instance.AvatarWeaponHandler.EquipWeapon(this.Id);
    }

    public override void ShopGUI()
    {
        base.ShopGUI();

        GUILayout.TextField("dps " + (DamagePerHit / Cooldown), GUILayout.Width(60));
    }

    public override void InventoryGUI()
    {
        base.InventoryGUI();

        if (MyAvatar.Instance.AvatarWeaponHandler != this && GUILayout.Button("Equip"))
            MyAvatar.Instance.AvatarWeaponHandler.EquipWeapon(this.Id);
        else if (GUILayout.Button("Unequip"))
            MyAvatar.Instance.AvatarWeaponHandler.UnEquipWeapon(this.Id);
    }

    [System.Serializable]
    public struct BaseWeaponValues
    {
        public int Seed;
        public float Damage;
        public float Cooldown;
        public Color WeaponColor;
        public float Range;

        public override string ToString ()
        {
            return Seed + ".) dam:" + Damage + "| CD:" + Cooldown + "|Range:" + Range;
        }
    }
}
