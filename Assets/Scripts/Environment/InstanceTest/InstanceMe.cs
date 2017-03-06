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
	
    void UpdateTransformMatrix()
    {
        moved = true;
        StartCoroutine(updateMyMatrix());
    }
    float magnitudeLimit = 0.1f;
    IEnumerator updateMyMatrix()
    {
        while(gameObject.activeInHierarchy)
        {
            yield return new WaitForFixedUpdate();
            if(rb.velocity.magnitude < magnitudeLimit)
                DrawInstanced.Instance.UpdateMatrix(transform, collection, Material);
        }
    }

    //private void FixedUpdate()
    //{
    //    if (moved)
    //    {
    //        if (rb != null && !Mathf.Approximately(rb.velocity.magnitude, 0f))
    //        {
    //            DrawInstanced.Instance.UpdateMatrix(transform, collection, Material);
    //        }
    //    }
    //}
}
