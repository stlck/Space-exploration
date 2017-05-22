using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour {

    public GenericComparitor<float> WeaponCooldownComparitor;
    public FloatUpdate WeaponCooldownEvent;
    //public BoolUpdate WeaponEquipped;
    public GenericComparitor<bool> WeaponEquipped;
    public BoolUpdate WeaponEquippedEvent;

    // Use this for initialization
    void Start () {
        WeaponCooldownComparitor = new GenericComparitor<float>();
        WeaponCooldownComparitor.ComparisonAction = () =>
        {
            if (MyAvatar.Instance.AvatarWeaponHandler.EquippedWeapon != null)
                WeaponCooldownComparitor.Compare(MyAvatar.Instance.AvatarWeaponHandler.EquippedWeapon.CurrentCooldown / MyAvatar.Instance.AvatarWeaponHandler.EquippedWeapon.WeaponValues.Cooldown);
        };
        WeaponCooldownComparitor.UpdateAction = (f) => WeaponCooldownEvent.Invoke(f);

        WeaponEquipped = new GenericComparitor<bool>();
        WeaponEquipped.ComparisonAction = () => WeaponEquipped.Compare( MyAvatar.Instance.AvatarWeaponHandler.EquippedWeapon != null);
        WeaponEquipped.UpdateAction = (b) => WeaponEquippedEvent.Invoke(b);
    }
	
	// Update is called once per frame
	void Update () {
        WeaponEquipped.ComparisonAction();
        WeaponCooldownComparitor.ComparisonAction();
    }
}
