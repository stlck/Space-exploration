﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using Isosurface;

[ExecuteInEditMode]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class DataProcessorExample : MonoBehaviour
{
	#region Public Properties

	public float speed = 1f;

	public Vector3 scale = Vector3.one;

	#endregion



	#region Hidden Properties

	float time;

	#endregion



	#region Main Methods

	void OnEnable ()
	{
#if UNITY_EDITOR
        EditorApplication.update += Update;
#endif
    }

	void Update ()
	{
		if (this == null || !this.isActiveAndEnabled)
			return;

#if UNITY_EDITOR
		if (!Application.isPlaying)
            time = (float)EditorApplication.timeSinceStartup;
		else
#endif
			time = Time.time;
    }

	#endregion



	#region Utility Methods

	// Sets the data equal the sin of each points' magnitude
	public void ProcessData (float[,,] data, Isosurface3D isosurfaceReference)
	{
		for (int x = 0; x < data.GetLength (0); x++)
			for (int y = 0; y < data.GetLength (1); y++)
				for (int z = 0; z < data.GetLength (2); z++)
					data[x, y, z] = Mathf.Sin (x * scale.x + Mathf.Sin (y * scale.y + Mathf.Sin (z * scale.z)) + (time * speed));

		isosurfaceReference.Data = data;
	}

	#endregion
}