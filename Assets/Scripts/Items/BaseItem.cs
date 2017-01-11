using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour {

    public int Price = 100;

    public virtual bool CanBuy()
    {
        return TeamStats.Instance.Credits >= Price;
    }

    public virtual void BuyItem()
    {
        MyAvatar.Instance.CmdAddCredits(-Price);
        MyAvatar.Instance.InventoryItems.Add(this);
    }

    public virtual void ShopGUI()
    {
        if (GUILayout.Button(name) && CanBuy())
            BuyItem();
        GUILayout.TextField("$" + Price, GUILayout.Width(60));
    }

    public virtual void InventoryGUI()
    {
        GUILayout.TextField(name);
    }
}
