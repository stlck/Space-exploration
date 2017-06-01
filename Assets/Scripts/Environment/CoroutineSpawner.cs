using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineSpawner : MonoBehaviour {

    private static CoroutineSpawner instance;
    public static CoroutineSpawner Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new GameObject("Coroutine Spawner").AddComponent<CoroutineSpawner>();
                Debug.Log("NEW COROUTINESPAWNER");
                instance.StartCoroutine(instance.SpawnRoutine());
            }
            return instance;
        }
    }

    public Queue<SpawnCase> SpawnQueue = new Queue<SpawnCase>();
    public float BlockSize = 1f;
    public float SpawnFrameLimit = .2f;
    public int InstantiateCount = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            StartCoroutine(instance.SpawnRoutine());
        }
    }

    public void Enqueue (Transform transform, Vector3 position, Quaternion q, Transform p, int layer = 8)
    {
        SpawnQueue.Enqueue(new SpawnCase( transform, position, p, BlockSize * Vector3.one, layer));
    }
    
    IEnumerator SpawnRoutine ()
    {
        float timer = 0f;
        float real = 0f;
        while (true)
        {
            real = Time.realtimeSinceStartup;
            while (SpawnQueue.Count > 0 && timer < SpawnFrameLimit)
            {
                timer = (Time.realtimeSinceStartup - real);
                var c = SpawnQueue.Dequeue();
                var t = Instantiate(c.t, c.Parent);
                t.gameObject.layer = c.Layer;
                t.localPosition = c.position * BlockSize;
                t.localScale = c.Size;
                InstantiateCount++;
            }
            yield return new WaitForEndOfFrame();
            timer = 0f;
        }
    }
    public struct SpawnCase
    {
        public SpawnCase(Transform T, Vector3 Position, Transform parent, Vector3 size, int layer)
        {
            t = T;
            position = Position;
            Parent = parent;
            Size = size;
            Layer = layer;
        }
        public Transform t;
        public Vector3 position;
        public Transform Parent;
        public Vector3 Size;
        public int Layer;
    }
}
