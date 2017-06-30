using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Reflection;
using Isosurface;

[System.Serializable]
public class DataProcessorEvent : UnityEvent<float[,,], Isosurface3D>
{
}


[AddComponentMenu ("Easy Isosurface/Mesh Isosurface")]
[RequireComponent (typeof (MeshFilter), typeof (MeshRenderer))]
[ExecuteInEditMode]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class IsosurfaceMesh : MonoBehaviour
{
	#region Public Properties

	public Isosurface3D isosurface;

	public DataProcessorEvent dataProcessor = new DataProcessorEvent ();

	public bool update = false;

	#endregion



	#region Hidden Properties

	MeshFilter meshFilter;
	MeshRenderer meshRenderer;

	Mesh editorMesh;
	public Mesh runtimeMesh;

	Camera editorCamera;

	#endregion



	#region Main Methods

	void Awake ()
	{
		Init ();
	}

	void OnEnable ()
	{
		if (!this.isActiveAndEnabled)
			return;

#if UNITY_EDITOR
        EditorApplication.update += Update;
#endif
		Init ();
    }

	void Init ()
	{
		if (meshFilter == null)
			meshFilter = GetComponent<MeshFilter> ();

		if (meshRenderer == null)
			meshRenderer = GetComponent<MeshRenderer> ();

		if (editorMesh == null)
			editorMesh = new Mesh ();

		if (Application.isPlaying)
		{
			runtimeMesh = new Mesh ();
			meshFilter.mesh = runtimeMesh;
		}

		UpdateMesh ();
	}

	void Update ()
	{
		if (this == null || !this.isActiveAndEnabled)
			return;

		UpdateMesh ();
	}

	void OnValidate ()
	{
		isosurface.Size.SetX (Mathf.Clamp (isosurface.Size.x, 1, 20));
		isosurface.Size.SetY (Mathf.Clamp (isosurface.Size.y, 1, 20));
		isosurface.Size.SetZ (Mathf.Clamp (isosurface.Size.z, 1, 20));

		isosurface.ResizeData (isosurface.Size);
	}

	#endregion



	#region Utility Methods

	void UpdateMesh ()
	{
		if (Application.isPlaying)
		{
			if (update)
				dataProcessor.Invoke (isosurface.Data, isosurface);

			DrawRuntimeMesh ();
		}
#if UNITY_EDITOR
        else
        {
			if (update && dataProcessor.GetPersistentEventCount () > 0)
				dataProcessor.GetPersistentTarget (0).GetType ().GetMethod (dataProcessor.GetPersistentMethodName (0)).Invoke (dataProcessor.GetPersistentTarget (0), new object[2]
				{
					isosurface.Data, isosurface
				});

            DrawEditorMesh ();
		}
	}

	void DrawEditorMesh ()
	{
		if (editorCamera == null && SceneView.lastActiveSceneView != null)
			editorCamera = SceneView.lastActiveSceneView.camera;

		if (update)
			isosurface.BuildData (ref editorMesh);

		if (editorMesh != null)
		{
			Graphics.DrawMesh (editorMesh, transform.position, transform.rotation, meshRenderer.sharedMaterial, 0, editorCamera);
			Graphics.DrawMesh (editorMesh, transform.position, transform.rotation, meshRenderer.sharedMaterial, 0, Camera.main);

			if (Camera.current != editorCamera && Camera.current != Camera.main)
				Graphics.DrawMesh (editorMesh, transform.position, transform.rotation, meshRenderer.sharedMaterial, 0, Camera.current);
        }
#endif
    }

    public void DrawRuntimeMesh ()
	{
		if (update)
			isosurface.BuildData (ref runtimeMesh);
	}

	#endregion
}