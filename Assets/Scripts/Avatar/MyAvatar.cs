using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Linq;

public class MyAvatar : NetworkBehaviour
{
    private static MyAvatar instance;
    public static MyAvatar Instance
    {
        get
        {
            return instance;
        }
    }

    public NetworkIdentity Identity;
    public ReadInput MyInput;
    public MoveAvator Movement;
    public MoveShip ShipMovement;
    public MovementBase CurrentMovementBase;
    public ServerCommandObject ServerCommands;
    public AvatarWeaponHandler AvatarWeaponHandler;
    public StatBase MyStats;
    public List<BaseItem> InventoryItems = new List<BaseItem>();

    //public List<Location> MyLocations = new List<Location>();
    //public List<Location> SpawnedLocations = new List<Location>();
    public CreateEnvironment EnvironmentCreator;

    [SyncVar]
    public int PlayerId;

    [SyncVar]
    public bool CanMove = true;

    [SyncVar]
    public States CurrentState;

    [SyncEvent]
    public event StateChange EventPlayerStateChanged;
    public delegate void StateChange(States newState);

    [SyncVar]
    public string test = "not set";

    public UnityEvent IsLocal;

    // Use this for initialization
    void Awake()
    {
        MyStats = GetComponent<StatBase>();
        InventoryItems = new List<BaseItem>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        instance = this;
        MyInput.AxisOut += updateMovement;
        MyInput.LookingAtPosition += updateLookingAt;

        IsLocal.Invoke();
    }

    void Start()
    {
        if (Identity.isServer)
        {
            CurrentState = States.Avatar;
            NetworkHelper.Instance.AllPlayers.Add(this);
            PlayerId = NetworkHelper.Instance.AllPlayers.Count;
            NetworkServer.SpawnObjects();
            transform.position = Vector3.up * .51f;

            if(!isLocalPlayer)
            {
                IsLocal.Invoke();
                MyInput.AxisOut += updateMovement;
                MyInput.LookingAtPosition += updateLookingAt;
            }
        }
    }

    void updateMovement(float v, float h)
    {
        if(isClient && CanMove)//CurrentState == States.Avatar)
        { 
            Movement.SetInput(v, h);
        }
        else if(isServer)
        {

            if (CurrentMovementBase != null)
                CurrentMovementBase.SetInput(v, h);
            else if( CanMove)
                Movement.SetInput(v, h);

            //if (CurrentState == States.Avatar)
            //{ 
            //    Movement.SetInput(v, h);
            //}
            //else if (CurrentState == States.OnShip && ShipMovement != null)
            //    ShipMovement.SetInput(v, h);
        }

    }

    void updateLookingAt(Vector3 t)
    {
        if (isClient && CanMove)//CurrentState == States.Avatar)
        {
            Movement.SetLookingAt(t);
        }
        else if (isServer)
        {
            if (CurrentMovementBase != null)
                CurrentMovementBase.SetLookingAt(t);
            else if (CanMove)
                Movement.SetLookingAt(t);
        }

    }

    void Update()
    {
        // set parent on new collider
        RaycastHit hit;
        if( Physics.Raycast(transform.position, transform.up * -1, out hit, 1, Movement.walkable))
        {
            if (hit.transform.root != transform.parent)
                SetParent(hit.transform.root);
        }

        if(isLocalPlayer)
        {
            if(CurrentState == States.OnShip && Input.GetKeyDown(KeyCode.Escape))
            {
                CmdReleaseShip();
            }
        }
    }

    public void SetParent(Transform newP)
    {
        transform.SetParent(newP, true);
    }

    public void SetMovementInput(MovementBase s)
    {
        CmdControlTarget(s.GetComponent<NetworkIdentity>().netId);
    }

    public void ReleaseControl()
    {
        CmdReleaseShip();
    }

    [Command]
    void CmdReleaseShip()
    {
        CurrentMovementBase = null;
        ChangeState(States.Avatar);
        CanMove = false;
    }

    [Command]
    void CmdControlTarget(NetworkInstanceId id)
    {
        if(CanMove)
        {
            CurrentMovementBase = NetworkServer.FindLocalObject(id).GetComponent<MovementBase>();
            ChangeState(States.OnShip);
            CanMove = false;
        }
        else
        {
            CurrentMovementBase.SetInput(0, 0);
            CurrentMovementBase.SetLookingAt(Vector3.zero);
            CurrentMovementBase = null;
            ChangeState(States.Avatar);
            CanMove = true;
        }
    }

    public void ChangeState(States newState)
    {
        Debug.Log("State update");
        CurrentState = newState;
        EventPlayerStateChanged(newState);
    }

    [Command]
    public void CmdAddCredits(int amount)
    {
        TeamStats.Instance.AddCredits(amount);
    }
}
