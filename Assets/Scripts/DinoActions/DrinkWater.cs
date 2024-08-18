using UnityEngine;

public class DrinkWater : GAction
{
    public float thirstModifier = 1;

    public override bool PrePerform()
    {
        FindClosestWaterIntersection();
        return true;
    }

    public override bool PostPerform()
    {
        gAgent.thirst -= 20 * thirstModifier;
        Destroy(gAgent.currentAction.target);
        gAgent.currentAction.target = null;

        Debug.Log("Finished Drinking");
        return true;
    }

    private void FindClosestWaterIntersection()
    {

        var vertices = terrainGeneration.mesh.vertices;
        var waterHeight = terrainGenerationData.waterHeight;
        var closestPoint = Vector3.positiveInfinity;
        var closestDistance = Mathf.Infinity;

        //Debug.Log($"Water Height: {waterHeight}");

        foreach (var vertex in vertices)
            // Determine if the vertex is close to the water height
            if (Mathf.Abs(vertex.y * terrainGenerationData.meshScale - waterHeight) < 2f) // Threshold can be adjusted
            {
                //Debug.Log($"Vertex near water height: {vertex}");
                // Calculate the distance from the AI agent
                var distance = Vector3.Distance(transform.position, vertex * terrainGenerationData.meshScale);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = vertex * terrainGenerationData.meshScale;
                    //Debug.Log($"New closest point: {closestPoint}, Distance: {closestDistance}");
                }
            }

        if (closestPoint != Vector3.positiveInfinity)
        {
            if (target != null)
            {
                // Update the target position
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
            Debug.LogWarning("No valid water intersection found.");
        }
    }
}