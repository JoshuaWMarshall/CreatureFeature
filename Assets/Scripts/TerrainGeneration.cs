using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    public Terrain terrain; // Reference to the terrain object in the scene

    void Start()
    {
        // Get the terrain data
        TerrainData terrainData = terrain.terrainData;

        // Generate random heights for the terrain
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        for (int i = 0; i < terrainData.heightmapResolution; i++)
        {
            for (int j = 0; j < terrainData.heightmapResolution; j++)
            {
                heights[i, j] = Random.Range(0.0f, 0.5f)/100; // Random height value between 0 and 1
            }
        }

        // Apply the generated heights to the terrain
        terrainData.SetHeights(0, 0, heights);
    }

    void Update()
    {
        // You can add terrain modification logic here if needed
    }
}
