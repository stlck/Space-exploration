using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceMe : MonoBehaviour
{
    public int collection { get; set; }
    bool moved;
    Vector3 position;
    Rigidbody rb;

    public Material Material;

    // for DrawIndirect
    public int X;
    public int Y;
    public int Z;

    void OnDisable()
    {
        DrawInstanced.Instance.RemoveFromDraw(transform, collection, Material);
    }

    // Use this for initialization
    void Start()
    {
        collection = DrawInstanced.Instance.AddToDraw(transform, Material);
        //rb = GetComponent<Rigidbody>();
    }

    //void UpdateTransformMatrix()
    //{
    //    moved = true;
    //    StartCoroutine(updateMyMatrix());
    //}

    //float magnitudeLimit = 0.1f;
    //IEnumerator updateMyMatrix()
    //{
    //    Debug.Log("Started updating transform Matrix");
    //    while (gameObject.activeInHierarchy)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        if (rb != null && rb.velocity.magnitude > magnitudeLimit)
    //            DrawInstanced.Instance.UpdateMatrix(transform, collection, Material);
    //    }
    //}
}
