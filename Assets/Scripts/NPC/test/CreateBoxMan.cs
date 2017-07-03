using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoxMan : NpcBase {

    [ContextMenuItem("Create","Create")]
    public Transform Prefab;
    public Vector3 Size;
    public float Scale;
    public float ReassemblySpeed = 5f;
    public float ReassemblyRotation = 5f;
    public List<BoxManPart> Parts = new List<BoxManPart>();

    public float ExternalExplosionModifier = 4f;
    public StatBase MyStats;
    
    public Transform Parent;
    public float Hittimer = 1f;

    // Use this for initialization
    void Start () {
        Create();
    }
	
    public void Create()
    {
        var c = GetComponent<Collider>();
        for (int i = 0; i < Size.x; i++)
            for (int j = 0; j < Size.y; j++)
                for (int k = 0; k < Size.z; k++)
                {
                    var p = new Vector3(i, j, k) * Scale ;
                    var t = Instantiate(Prefab, Parent);
                    t.gameObject.SetActive(true);
                    t.position = p + transform.position;
                    t.localScale = Vector3.one * Scale * .98f;
                    Parts.Add(new BoxManPart()
                    {
                        Target = t,
                        TargetLocalPosition = p,
                        rigidBody = t.GetComponent<Rigidbody>()
                    });
                    Physics.IgnoreCollision(c, t.GetComponent<Collider>());
                    var cm = t.gameObject.AddComponent<CollisionMessage>();
                    cm.CollisionEntered += Cm_CollisionEntered;
                }
    }

    private void Cm_CollisionEntered(Collision col)
    {

    }

    // Update is called once per frame
    public override void Update () {
        base.Update();

        realignParts();
	}
    public override void Chase(Vector3 position)
    {
        base.Chase(position);

        //transform.LookAt(stateTarget);
        var from = transform.position + new Vector3(
                    Random.Range(0, Size.x * Scale),
                    Random.Range(0, Size.y * Scale),
                    Random.Range(0, Size.z * Scale));
        //for (int i = 0; i < Parts.Count; i++)
        ApplyForce(
            from/*+ Scale * Size / 2 + Random.Range(-1.5f,1.5f) * Vector3.up*/,
            ExternalExplosionModifier,
            4,
            0f);//Parts[i].rigidBody.AddExplosionForce(200, transform.position - transform.forward, 6);

        transform.position = position;
    }

    void realignParts()
    {
        for (int i = 0; i < Parts.Count; i++)
        {
            if (Parts[i].PartHit)
            {
                Parts[i].HitTimer += Time.deltaTime;
                if (Parts[i].HitTimer >= Hittimer)
                {
                    Parts[i].PartHit = false;
                    Parts[i].HitTimer = 0f;
                }
            }

            if (!Parts[i].PartHit)
            {
                Parts[i].rigidBody.angularVelocity = Vector3.zero;
                Parts[i].rigidBody.velocity = Vector3.Lerp(Parts[i].rigidBody.velocity, Vector3.zero, Time.deltaTime * ReassemblySpeed);
                Parts[i].MoveSpeed = Mathf.Lerp(Parts[i].MoveSpeed, ReassemblySpeed, Time.deltaTime);
                Parts[i].Target.position = Vector3.MoveTowards(Parts[i].Target.position, transform.TransformPoint( Parts[i].TargetLocalPosition), Time.deltaTime * Parts[i].MoveSpeed);
                Parts[i].Target.rotation = Quaternion.RotateTowards(Parts[i].Target.rotation, Quaternion.identity, Time.deltaTime * ReassemblyRotation);

                if (Parts[i].MoveSpeed > 1f && Vector3.Distance(Parts[i].Target.localPosition, Parts[i].TargetLocalPosition) < .1f)
                {
                    Parts[i].MoveSpeed = 0f;
                    Parts[i].Target.position = transform.TransformPoint(Parts[i].TargetLocalPosition);// Parts[i].TargetLocalPosition;
                }
            }
        }
    }

    public override void ApplyForce(Vector3 origin, float force, float radius, float damage)
    {
        for (int i = 0; i < Parts.Count; i++)
        {
            Parts[i].rigidBody.AddExplosionForce(force * ExternalExplosionModifier, origin, radius);
            Parts[i].HitTimer = 0f;
            Parts[i].MoveSpeed = 0;
            Parts[i].PartHit = true;
        }
    }

    public class BoxManPart
    {
        public Transform Target;
        public Rigidbody rigidBody;
        public float MoveSpeed = 0;
        public bool PartHit = false;
        public float HitTimer = 0f;
        public Vector3 TargetLocalPosition;
    }
}
