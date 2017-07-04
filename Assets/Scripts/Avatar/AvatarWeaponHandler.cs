using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarWeaponHandler : NetworkBehaviour
{
    public BaseWeapon EquippedWeapon;
    public List<WeaponGenerator.InstantiatedWeapon> InstantiatedWeapons = new List<WeaponGenerator.InstantiatedWeapon>();
    public Transform WeaponPoint;

    void Awake()
    {
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        MyAvatar.Instance.AvatarWeaponHandler = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public void UnEquipWeapon(int seed)
    {
        CmdUnequipWeapon(seed);
    }

    //public void EquipWeapon(int id)
    //{
    //    CmdEquipWeapon(id);
    //}

    public void EquipWeaponSeed ( int seed)
    {
        CmdEquipWeaponSeed( seed);
    }

    [Command]
    public void CmdEquipWeaponSeed ( int seed)
    {
        RpcEquipWeaponSeed( seed);
    }

    [ClientRpc]
    void RpcEquipWeaponSeed ( int seed)
    {
        Debug.Log("Equipping : " + seed + ". in collection : " + string.Join(" ", InstantiatedWeapons.Select(m => m.Seed.ToString()).ToArray()));
        turnOffEquippedWeapon();
        //Destroy(EquippedWeapon.gameObject);
        if(!InstantiatedWeapons.Any(m => m.Seed == seed))
        {
            var recipee = WeaponGenerator.GetRecipee(seed);
            EquippedWeapon = WeaponGenerator.InstantiateWeapon(recipee.OrgWeapon, WeaponPoint);
            EquippedWeapon.WeaponValues = WeaponGenerator.getBaseWeaponValues(recipee);
            InstantiatedWeapons.Add(new WeaponGenerator.InstantiatedWeapon() { GameObject = EquippedWeapon.gameObject, Id = EquippedWeapon.Id, Seed = seed, Weapon = EquippedWeapon });
            Testing.AddDebug("Instantiating " + recipee.OrgWeapon.name + "  with seed : " + seed);
        }
        else
        {
            EquippedWeapon = InstantiatedWeapons.First(m => m.Seed == seed).Weapon;
            EquippedWeapon.gameObject.SetActive(true);
        }
    }

    //[Command]
    //void CmdEquipWeapon(int id)
    //{
    //    RpcUpdateWeaponInfo(id);
    //}

    void turnOffEquippedWeapon()
    {
        if (EquippedWeapon != null)
            EquippedWeapon.gameObject.SetActive(false);
    }

    //[ClientRpc]
    //void RpcUpdateWeaponInfo(int id)
    //{
    //    turnOffEquippedWeapon();

    //    var w = WeaponGenerator.InstantiateWeapon(id, WeaponPoint);
        
    //    EquippedWeapon = w;
    //}

    [Command]
    void CmdUnequipWeapon(int seed)
    {
        RpcUnequipweapon(seed);
        turnOffEquippedWeapon();
        EquippedWeapon = null;
    }

    [ClientRpc]
    void RpcUnequipweapon(int seed)
    {
        turnOffEquippedWeapon();
        EquippedWeapon = null;
    }

    public void TryFire()
    {
        if(EquippedWeapon != null && EquippedWeapon.CanFire() && MyAvatar.Instance.CurrentState == States.Avatar)
            CmdFireWeapon();
    }

    public void ServerFire()
    {
        if(EquippedWeapon != null && EquippedWeapon.CanFire())
        { 
            RpcFireWeapon();
        }
    }

    [Command]
    void CmdFireWeapon()
    {
        RpcFireWeapon();
        EquippedWeapon.FireWeapon();
    }

    [ClientRpc]
    void RpcFireWeapon()
    {
        EquippedWeapon.FireWeapon();
    }

}

