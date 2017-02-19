using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceMe : MonoBehaviour {

    int collection;
    bool moved;
    Vector3 position;
    Rigidbody rb;

    public Material Material;

    void OnDisable()
    {
        DrawInstanced.Instance.RemoveFromDraw(transform, collection, Material);
    }

	// Use this for initialization
	void Start () {
        collection = DrawInstanced.Instance.AddToDraw(transform, Material);
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
		if(position != transform.position)
        {
            //DrawInstanced.Instance.UpdateMatrix(transform, collection, Material);
            //position = transform.position;
        }
	}

    private void FixedUpdate()
    {
        if(rb != null && !Mathf.Approximately(rb.velocity.magnitude, 0f))
        {
            DrawInstanced.Instance.UpdateMatrix(transform, collection, Material);
        }
    }
}
