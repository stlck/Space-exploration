using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarWeaponHandler : NetworkBehaviour
{
    public BaseWeapon EquippedWeapon;
    public List<InstantiatedWeapon> InstantiatedWeapons = new List<InstantiatedWeapon>();
    public static List<BaseWeapon> LoadedWeapons = new List<BaseWeapon>();
    public Transform WeaponPoint;

    void Awake()
    {
        LoadedWeapons = Resources.LoadAll<BaseWeapon>("Weapons").ToList();
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

    public void EquipWeapon(int id)
    {
        CmdEquipWeapon(id);
    }

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
        turnOffEquippedWeapon();
        //Destroy(EquippedWeapon.gameObject);
        if(!InstantiatedWeapons.Any(m => m.Seed == seed))
        { 
            var recipee = WeaponGenerator.GetRecipee(seed);
            EquippedWeapon = instantiateWeapon(recipee.OrgWeapon.Id, seed);
            EquippedWeapon.WeaponValues = WeaponGenerator.getBaseWeaponValues(recipee);
        }
        else
        {
            EquippedWeapon = InstantiatedWeapons.First(m => m.Seed == seed).Weapon;
            EquippedWeapon.gameObject.SetActive(true);
        }
    }

    void OnGUI()
    {
        if(isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(Screen.width/2 - 100, Screen.height - 55, 200, 50));
            GUILayout.Label(GetComponentsInChildren<BaseWeapon>().Count() + " weapons");
            if(EquippedWeapon != null)
            {
                GUILayout.Button(EquippedWeapon.name + " : " + (int) Mathf.Min(100,100 * EquippedWeapon.CurrentCooldown / EquippedWeapon.WeaponValues.Cooldown));
            }
            GUILayout.EndArea();
        }
    }

    [Command]
    void CmdEquipWeapon(int id)
    {
        RpcUpdateWeaponInfo(id);
    }

    void turnOffEquippedWeapon()
    {
        if (EquippedWeapon != null)
            EquippedWeapon.gameObject.SetActive(false);
    }

    [ClientRpc]
    void RpcUpdateWeaponInfo(int id)
    {
        turnOffEquippedWeapon();
        //Destroy(EquippedWeapon.gameObject);

        var w = instantiateWeapon(id, id);
        
        EquippedWeapon = w;
    }

    BaseWeapon instantiateWeapon (int id, int seed)
    {
        BaseWeapon w = Instantiate(LoadedWeapons.First(m => m.Id == id));

        w.transform.parent = WeaponPoint;
        w.transform.localPosition = w.transform.position;
        w.transform.localEulerAngles = Vector3.zero;

        InstantiatedWeapons.Add(new InstantiatedWeapon() { GameObject = w.gameObject, Id = id, Seed = seed, Weapon = w });

        return w;
    }

    [Command]
    void CmdUnequipWeapon(int seed)
    {
        //NetworkServer.Destroy(EquippedWeapon.gameObject);
        RpcUnequipweapon(seed);
        turnOffEquippedWeapon();
        //Destroy(EquippedWeapon.gameObject);
        EquippedWeapon = null;
    }

    [ClientRpc]
    void RpcUnequipweapon(int seed)
    {
        turnOffEquippedWeapon();
        //Destroy(EquippedWeapon.gameObject);
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
            //EquippedWeapon.FireWeapon();
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
        Debug.Log("fIRING");
        EquippedWeapon.FireWeapon();
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

