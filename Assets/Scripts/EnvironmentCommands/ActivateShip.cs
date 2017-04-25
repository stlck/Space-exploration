using UnityEngine;
using System.Collections;
using System;
using System.Linq;

//[RequireComponent(typeof(CommandObject))]
public class ActivateShip : MonoBehaviour//, CmdObj
{
    public Ship TargetShip;
    MovementBase shipMovement;
    void Start()
    {
        TargetShip = GetComponentInParent<Ship>();
        shipMovement = TargetShip.GetComponent<MovementBase>();
    }

    public void OnMouseUp()
    {
        if(Vector3.Distance(transform.position, MyAvatar.Instance.transform.position) < 3)
            MyAvatar.Instance.SetMovementInput(TargetShip.GetComponent<MovementBase>());
        else
            RangeIndicator.Instance.TurnOn(transform.position, 3);
    }
}
