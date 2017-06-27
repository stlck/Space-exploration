using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class BaseWeapon : BaseItem
{
    public int Id = -1;
    public float CurrentCooldown = 0f;
    public ColorUpdate WeaponColor;
    public BaseWeaponValues WeaponValues;
    public List<ParticleSystem> ParticleSystems;

    // Use this for initialization
    public virtual void Start () {
        ParticleSystems.ForEach(m =>
        {
            var main = m.main;
            main.startColor = WeaponValues.WeaponColor;
        });

        WeaponColor.Invoke(WeaponValues.WeaponColor);
	}

    // Update is called once per frame
    public virtual void Update () {
        CurrentCooldown += Time.deltaTime;
	}

    public virtual bool CanFire()
    {
        return CurrentCooldown >= WeaponValues.Cooldown;
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

        MyAvatar.Instance.AvatarWeaponHandler.EquipWeaponSeed(this.WeaponValues.Seed);
    }

    public void BuyWeapon(int seed)
    {
        base.BuyItem();
        
        MyAvatar.Instance.AvatarWeaponHandler.EquipWeaponSeed(seed);   
    }

    public override void ShopGUI()
    {
        base.ShopGUI();

        GUILayout.TextField("seed: " + WeaponValues.Seed + ". dps " + (WeaponValues.Damage / WeaponValues.Cooldown), GUILayout.Width(60));
    }

    public override void InventoryGUI()
    {
        base.InventoryGUI();

        if (MyAvatar.Instance.AvatarWeaponHandler != this && GUILayout.Button("Equip " + WeaponValues.Seed))
            MyAvatar.Instance.AvatarWeaponHandler.EquipWeaponSeed(this.WeaponValues.Seed);
        else if (GUILayout.Button("Unequip"))
            MyAvatar.Instance.AvatarWeaponHandler.UnEquipWeapon(this.WeaponValues.Seed);
    }

    [System.Serializable]
    public struct BaseWeaponValues
    {
        //public bool IsSetup;
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
