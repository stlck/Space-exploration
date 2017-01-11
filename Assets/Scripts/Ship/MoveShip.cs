using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MoveShip : MovementBase
{
    Ship owningShip;

    public float MoveSpeed = 5;
    public float RotateSpeed = 3;

    void Awake()
    {
        owningShip = GetComponentInParent<Ship>();
    }

    public void ActivateShip(ReadInput i)
    {

    }

    public void DeactivateShip()
    {
        vert = hor = 0;
    }

    void OnDisable()
    {
        vert = hor = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!owningShip.Warping && isServer)
        {
            //owningShip.CurrentSpeed = Mathf.Lerp(owningShip.CurrentSpeed, MoveSpeed, Time.deltaTime);
            transform.Translate(Vector3.forward * vert * Time.deltaTime * MoveSpeed);// owningShip.CurrentSpeed);
            transform.Rotate(Vector3.up, hor * Time.deltaTime * RotateSpeed);
            // check boundaries
        }
    }
}
