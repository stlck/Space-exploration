﻿using System.Collections;
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
                instance.StartCoroutine(instance.SpawnRoutine());
            }
            return instance;
        }
    }

    public Queue<SpawnCase> SpawnQueue = new Queue<SpawnCase>();
    public float BlockSize = 1f;
    public float SpawnFrameLimit = .2f;
    public int InstantiateCount = 0;

    public void Enqueue (Transform transform, Vector3 position, Quaternion q, Transform p)
    {
        SpawnQueue.Enqueue(new SpawnCase( transform, position, p, BlockSize * Vector3.one));
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
                var t = Instantiate(c.t, c.position * BlockSize, Quaternion.identity, c.Parent);
                t.localScale = c.Size;
                InstantiateCount++;
            }
            yield return new WaitForEndOfFrame();
            timer = 0f;
        }
    }
    public struct SpawnCase
    {
        public SpawnCase(Transform T, Vector3 Position, Transform parent, Vector3 size)
        {
            t = T;
            position = Position;
            Parent = parent;
            Size = size;
        }
        public Transform t;
        public Vector3 position;
        public Transform Parent;
        public Vector3 Size;
    }
}
