using UnityEngine;

public class DrinkWater : GAction
{
    // Modifier to adjust the amount of thirst quenched
    public float thirstModifier = 1;

    // Method called before the action is performed
    public override bool PrePerform()
    {
        // Find the closest water intersection point
        FindClosestWaterIntersection();
        return true;
    }

    // Method called after the action is performed
    public override bool PostPerform()
    {
        // Reduce the agent's thirst by a calculated amount
        gAgent.thirst -= 20 * thirstModifier;
        
        // Destroy the target object after drinking
        Destroy(gAgent.currentAction.target);
        
        // Clear the current action's target
        gAgent.currentAction.target = null;

        // Log the completion of the drinking action
        Debug.Log("Finished Drinking");
        return true;
    }

    // Method to find the closest water intersection point
    private void FindClosestWaterIntersection()
    {
        // Get the vertices of the terrain mesh
        var vertices = terrainGeneration.mesh.vertices;
        
        // Get the water height from the terrain data
        var waterHeight = terrainGenerationData.waterHeight;
        
        // Initialize variables to track the closest point and distance
        var closestPoint = Vector3.positiveInfinity;
        var closestDistance = Mathf.Infinity;

        // Iterate through each vertex in the terrain mesh
        foreach (var vertex in vertices)
        {
            // Determine if the vertex is close to the water height
            if (Mathf.Abs(vertex.y * terrainGenerationData.meshScale - waterHeight) < 2f) // Threshold can be adjusted
            {
                // Calculate the distance from the AI agent to the vertex
                var distance = Vector3.Distance(transform.position, vertex * terrainGenerationData.meshScale);
                
                // Update the closest point and distance if this vertex is closer
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = vertex * terrainGenerationData.meshScale;
                }
            }
        }

        // Check if a valid closest point was found
        if (closestPoint != Vector3.positiveInfinity)
        {
            if (target != null)
            {
                // Update the target position to the closest point
                target.transform.position = closestPoint;
            }
            else
            {
                // Create a new target GameObject if none exists
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