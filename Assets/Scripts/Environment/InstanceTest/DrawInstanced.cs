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

    //public List<Dictionary<Transform,Matrix4x4>> ToDraw = new List<Dictionary<Transform, Matrix4x4>>();
    Dictionary<Material, List<DrawContainer>> allContainers = new Dictionary<Material, List<DrawContainer>>();
    
    public Mesh TargetMesh;

    int currentIndex = 0;
    MaterialPropertyBlock propBlock;
    void Awake()
    {
        instance = this;
        allContainers = new Dictionary<Material, List<DrawContainer>>();
        propBlock = new MaterialPropertyBlock();
        //ToDraw.Add(new Dictionary<Transform, Matrix4x4>());
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        foreach(var c in allContainers)
        {
            foreach(var l in c.Value)
            {
                if(l.Updated)
                {
                    l.Matrixes = l.ToDraw.Values.ToArray();
                    l.Updated = false;
                }

                Graphics.DrawMeshInstanced(TargetMesh, 0, c.Key, l.Matrixes, l.Matrixes.Count(), propBlock, UnityEngine.Rendering.ShadowCastingMode.On,true,l.Layer);
                
            }
        }
           
	}

    public int AddToDraw(Transform addMe, Material mat = null)
    {
        if(!allContainers.ContainsKey(mat))
        {
            var newContainers = new List<DrawContainer>();
            var newContainer = new DrawContainer(mat, addMe.gameObject.layer);
            newContainer.AddToContainer(addMe);
            newContainers.Add(newContainer);
            allContainers.Add(mat, newContainers);
            return 0;
        }
        else if(allContainers[mat].Last().Count > 1020)
        {
            var newContainer = new DrawContainer(mat, addMe.gameObject.layer);
            newContainer.AddToContainer(addMe);
            allContainers[mat].Add(newContainer);
            return allContainers[mat].Count() -1;
        }
        else
        {
            allContainers[mat].Last().AddToContainer(addMe);
            return allContainers[mat].Count() - 1;
        }
    }

    public void UpdateMatrix(Transform update, int collection, Material mat = null)
    {
        //ToDraw[collection][update] = update.localToWorldMatrix;
        allContainers[mat][collection].UpdateMatrix(update);
    }

    public void RemoveFromDraw(Transform removeMe, int collection, Material mat = null)
    {
        //ToDraw[collection].Remove(removeMe);
        allContainers[mat][collection].RemoveFromDraw(removeMe);
    }

    public class DrawContainer
    {
        public Material TargetMaterial;
        public Dictionary<Transform, Matrix4x4> ToDraw;
        public int Layer;
        public int Count = 0;
        public bool Updated = false;
        public Matrix4x4[] Matrixes;
        int currentIndex = 0;

        public DrawContainer(Material mat, int l)
        {
            TargetMaterial = mat;
            Layer = l;
            ToDraw = new Dictionary<Transform, Matrix4x4>();
            //ToDraw.Add(new Dictionary<Transform, Matrix4x4>());
        }

        public void AddToContainer(Transform addMe)
        {
            ToDraw/*[currentIndex]*/.Add(addMe, addMe.localToWorldMatrix);
            Count++;
            Updated = true;
        }

        public void UpdateMatrix(Transform update)
        {
            ToDraw[update] = update.localToWorldMatrix;
            Updated = true;
        }

        public void RemoveFromDraw(Transform removeMe)
        {
            ToDraw.Remove(removeMe);
            Count--;
            Updated = true;
        }
    }
}
