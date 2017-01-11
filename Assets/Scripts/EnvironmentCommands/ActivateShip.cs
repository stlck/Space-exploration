using UnityEngine;
using System.Collections;
using System;
using System.Linq;

//[RequireComponent(typeof(CommandObject))]
public class ActivateShip : MonoBehaviour//, CmdObj
{
    public Ship TargetShip;

    void Start()
    {
        TargetShip = GetComponentInParent<Ship>();
    }

    /*
        public void doCommand(int senderId)
        {

            if(TargetShip.ControlledBy == -1)
            { 
                var t = NetworkHelper.Instance.AllPlayers.First(M => M.PlayerId == senderId);

                TargetShip.ControlledBy = senderId;
                //t.Movement.DisableMovement();
                TargetShip.ShipMovement.ActivateShip(t.MyInput);
            }
            else
            {
                var t = NetworkHelper.Instance.AllPlayers.First(M => M.PlayerId == senderId);
                TargetShip.ShipMovement.DeactivateShip();
                //t.Movement.EnableMovement();
            }
    }*/

    /*public void localCommand()
    {
        MyAvatar.Instance.TakeControlOffShip(TargetShip);
        if(TargetShip.ControlledBy == -1)
            MyAvatar.Instance.Movement.DisableMovement();
        else
            MyAvatar.Instance.Movement.EnableMovement();

}*/
    public void OnMouseUp()
    {
        MyAvatar.Instance.SetMovementInput(TargetShip.GetComponent<MovementBase>());
        /*switch (PlayerState.Instance.CurrentState)
        {
            case States.Avatar:
                PlayerState.Instance.ChangeState(States.OnShip);
                TargetShip.ActivateShip();
                // set playerparent
                //MyAvatar.Instance.SetParent(transform.root);
                break;
            case States.Dead:
                break;
            case States.OnShip:
                PlayerState.Instance.ChangeState(States.Avatar);
                TargetShip.DeActivateShip();
                // reset playerParent
                //MyAvatar.Instance.SetParent(null);
                break;
        }*/
    }
}
