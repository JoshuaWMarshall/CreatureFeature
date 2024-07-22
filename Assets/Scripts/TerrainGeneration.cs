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

        // Generate random heights for the terrain with fall-off effect
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        float center = terrainData.heightmapResolution / 2f; // Calculate the center of the terrain

        for (int i = 0; i < terrainData.heightmapResolution; i++)
        {
            for (int j = 0; j < terrainData.heightmapResolution; j++)
            {
                float distanceFromCenter = Vector2.Distance(new Vector2(i, j), new Vector2(center, center));
                float normalizedDistance = distanceFromCenter / center; // Normalize the distance

                // Apply fall-off effect to the height based on the normalized distance
                float fallOff = Mathf.Clamp01(1f - normalizedDistance);
                heights[i, j] = Random.Range(0.0f, 0.05f) * fallOff; // Apply fall-off to the random height
            }
        }

        // Apply the generated heights with fall-off effect to the terrain
        terrainData.SetHeights(0, 0, heights);
    }

    void Update()
    {
        // You can add terrain modification logic here if needed
    }
}
