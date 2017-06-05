using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class MoveShip : MovementBase
{
    Ship owningShip;
    Rigidbody rigidbody;
    public float MoveSpeed = 5;
    float lerpVert = 0f;
    public float RotateSpeed = 3;
    float lerpHor = 0f;

    public bool TryPhysics = true;
    public float PhysicsModifier = 200;

    public ShipCollisions CurrentCollisions;

    void Awake()
    {
        owningShip = GetComponentInParent<Ship>();
        rigidbody = GetComponent<Rigidbody>();
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
        lerpVert = Mathf.Lerp(lerpVert, (vert > 0) ? 1 : ((vert < 0) ? -1 : 0), Time.deltaTime);
        lerpHor = Mathf.Lerp(lerpHor, (hor > 0) ? 1 : ((hor < 0) ? -1 : 0), Time.deltaTime);

        if (!TryPhysics && !owningShip.Warping && isServer)
        {
            //owningShip.CurrentSpeed = Mathf.Lerp(owningShip.CurrentSpeed, MoveSpeed, Time.deltaTime);
            if (CurrentCollisions.CurrentCollisionNormal.Any())
            {
                var target = CurrentCollisions.CurrentCollisionNormal.First() * -1;
                target.y = 0f;
                transform.Translate( target * MoveSpeed * Time.deltaTime);
                // add collision damage & effects
            }
            else
                transform.Translate(Vector3.forward * lerpVert * Time.deltaTime * MoveSpeed);// owningShip.CurrentSpeed);
        }

        transform.Rotate(Vector3.up, lerpHor * Time.deltaTime * RotateSpeed);
    }

    void FixedUpdate()
    {
        if (TryPhysics && !owningShip.Warping && isServer)
        {
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.mass = 40;
            }

            rigidbody.AddForce(transform.forward * vert * MoveSpeed * PhysicsModifier * Time.fixedDeltaTime, ForceMode.VelocityChange);

            //rigidbody.AddTorque(Vector3.up * RotateSpeed * hor * Time.fixedDeltaTime * PhysicsModifier);

            if (rigidbody.velocity.magnitude > MoveSpeed)
                rigidbody.velocity = rigidbody.velocity.normalized * MoveSpeed; 
            // velocity limit
        }
    }
}
