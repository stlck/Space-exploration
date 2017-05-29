using UnityEngine;
using System.Collections;
using Isosurface;

public class NoiseExample : MonoBehaviour
{
	#region Public Properties

	public Vector3 scale = Vector3.one;

	public Vector3 pan = Vector3.zero;

    #endregion

    int[,,] stationMap;

    private void Awake()
    {
        
    }

    #region Utility Methods

    void buildstation(float[,,] data)
    {
        StationSpawner stationspawn = new StationSpawner();
        var map = stationspawn.Generate(transform, Random.Range(0, 10000), data.GetLength(0), 5, 8, 3, TileSet.BlueStation, false);

        int[,,] voxelMap = new int[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
        for (int i = 0; i < data.GetLength(0); i++)
            for (int j = 0; j < data.GetLength(2); j++)
                for (int k = 0; k < data.GetLength(1); k++)
                {
                    if (map[i, j] == 0 || 
                        map[i, j] == 1 && k >= 2 || 
                        map[i, j] == 2 && k >= 5)
                        voxelMap[i, k, j] = -1;
                    else if (map[i, j] == 1 || 
                        map[i, j] == 2)
                        voxelMap[i, k, j] = 1;
                    else
                        voxelMap[i, k, j] = 0;
                }
        stationMap = voxelMap;
    }

    public void ProcessData (float[,,] data, Isosurface3D isosurfaceReference)
	{
        if (stationMap == null)
            buildstation(data);

		for (int x = 0; x < data.GetLength (0); x++)
		{
			for (int y = 0; y < data.GetLength (1); y++)
			{
				for (int z = 0; z < data.GetLength (2); z++)
				{
                    //float sample = (x * scale.x + pan.x) + (y * scale.y + pan.y) + (z * scale.z + pan.z);
                    //float isample = (x * scale.z + pan.x) + (y * scale.x + pan.y) + (z * scale.y + pan.z);

                    data[x, y, z] = stationMap[x, y, z];// Mathf.PerlinNoise (sample, isample) - 0.5f;
				}
			}
		}

		isosurfaceReference.Data = data;
	}

	#endregion
}