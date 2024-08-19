using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWater : GAction
{
    // This method is called before the action is performed
    public override bool PrePerform()
    {
        // Find the closest water intersection point
        FindClosestWaterIntersection();
        return true;
    }

    // This method is called after the action is performed
    public override bool PostPerform()
    {
        // Currently, no post-perform actions are defined
        return true;
    }

    // Finds the closest intersection point with water on the terrain
    private void FindClosestWaterIntersection()
    {
        // Get the vertices of the terrain mesh
        Vector3[] vertices = terrainGeneration.mesh.vertices;
        // Get the water height from the terrain generation data
        float waterHeight = terrainGenerationData.waterHeight;
        // Initialize the closest point to a very large value
        Vector3 closestPoint = Vector3.positiveInfinity;
        // Initialize the closest distance to infinity
        float closestDistance = Mathf.Infinity;

        // Iterate through each vertex in the terrain mesh
        foreach (Vector3 vertex in vertices)
        {
            // Determine if the vertex is close to the water height
            if (Mathf.Abs(vertex.y * terrainGenerationData.meshScale - waterHeight) < 2f) // Threshold can be adjusted
            {
                // Calculate the distance from the AI agent to the vertex
                float distance = Vector3.Distance(transform.position, vertex * terrainGenerationData.meshScale);
                // Check if this vertex is closer than the previously found closest vertex
                if (distance < closestDistance)
                {
                    // Update the closest distance and point
                    closestDistance = distance;
                    closestPoint = vertex * terrainGenerationData.meshScale;
                }
            }
        }

        // If a valid closest point was found
        if (closestPoint != Vector3.positiveInfinity)
        {
            if (target != null)
            {
                // Update the target position to the closest point
                target.transform.position = closestPoint;
            }
            else
            {
                // Create a new target GameObject if none exists and set its position
                target = new GameObject("WaterTarget");
                target.transform.position = closestPoint;
            }
        }
        else
        {
            // Log a warning if no valid water intersection was found
            Debug.LogWarning("No valid water intersection found.");
        }
    }
}
