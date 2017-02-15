using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawInstanced : MonoBehaviour {

    private static DrawInstanced instance;
    public static DrawInstanced Instance
    {
        get
        {
            //if (instance == null)
            //    instance = new GameObject("GpuInstance").AddComponent<DrawInstanced>();
            return instance;
        }
    }

    public List<Dictionary<Transform,Matrix4x4>> ToDraw = new List<Dictionary<Transform, Matrix4x4>>();
    
    public Mesh TargetMesh;
    public Material TargetMaterial;

    int currentIndex = 0;

    void Awake()
    {
        instance = this;
        ToDraw.Add(new Dictionary<Transform, Matrix4x4>());
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
		//foreach(var l in ToDraw)
        for(int i = 0; i < ToDraw.Count; i++)
        {
            Graphics.DrawMeshInstanced(TargetMesh, 0, TargetMaterial, ToDraw[i].Values.ToArray());
        }
	}

    public int AddToDraw(Transform addMe)
    {
        if (ToDraw[currentIndex].Count > 1020)
        {
            ToDraw.Add(new Dictionary<Transform, Matrix4x4>());
            currentIndex++;
        }

        ToDraw[currentIndex].Add(addMe, addMe.localToWorldMatrix);

        return currentIndex;
    }

    public void UpdateMatrix(Transform update, int collection)
    {
        ToDraw[collection][update] = update.localToWorldMatrix;
    }

    public void RemoveFromDraw(Transform removeMe, int collection)
    {
        ToDraw[collection].Remove(removeMe);
    }
}
