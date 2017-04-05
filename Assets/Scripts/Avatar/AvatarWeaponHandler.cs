using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class AvatarWeaponHandler : NetworkBehaviour
{
    public BaseWeapon EquippedWeapon;

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

    public void UnEquipWeapon(int id)
    {
        CmdUnequipWeapon(id);
    }

    public void EquipWeapon(int id)
    {
        CmdEquipWeapon(id);
    }

    void OnGUI()
    {
        if(isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(Screen.width/2 - 100, Screen.height - 55, 200, 50));
            GUILayout.Label(GetComponentsInChildren<BaseWeapon>().Count() + " weapons");
            if(EquippedWeapon != null)
            {
                GUILayout.Button(EquippedWeapon.name + " : " + (int) Mathf.Min(100,100 * EquippedWeapon.CurrentCooldown / EquippedWeapon.Cooldown));
            }
            GUILayout.EndArea();
        }
    }

    [Command]
    void CmdEquipWeapon(int id)
    {
        //BaseWeapon w = Instantiate(LoadedWeapons.First(m => m.Id == id));
        ////NetworkServer.Spawn(w.gameObject);
        //w.transform.parent = transform;
        //w.transform.localPosition = w.transform.position;
        //w.transform.localEulerAngles = Vector3.zero;
        //EquippedWeapon = w;

        RpcUpdateWeaponInfo(id);
    }

    [ClientRpc]
    void RpcUpdateWeaponInfo(int id)
    {
        BaseWeapon w = Instantiate(LoadedWeapons.First(m => m.Id == id));
        //NetworkServer.Spawn(w.gameObject);
        w.transform.parent = WeaponPoint;
        w.transform.localPosition = w.transform.position;
        w.transform.localEulerAngles = Vector3.zero;

        EquippedWeapon = w;
    }

    [Command]
    void CmdUnequipWeapon(int id)
    {
        NetworkServer.Destroy(EquippedWeapon.gameObject);
        RpcUnequipweapon(id);
        EquippedWeapon = null;
    }

    [ClientRpc]
    void RpcUnequipweapon(int id)
    {
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
}

