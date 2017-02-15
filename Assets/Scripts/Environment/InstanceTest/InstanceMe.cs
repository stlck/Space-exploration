using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceMe : MonoBehaviour {

    int collection;
    bool moved;
    Vector3 position;

    public Material Material;

    void OnDisable()
    {
        DrawInstanced.Instance.RemoveFromDraw(transform, collection, Material);
    }

	// Use this for initialization
	void Start () {
        collection = DrawInstanced.Instance.AddToDraw(transform, Material);
    }
	
	// Update is called once per frame
	void Update () {
		if(position != transform.position)
        {
            DrawInstanced.Instance.UpdateMatrix(transform, collection, Material);
            position = transform.position;
        }
	}
}
