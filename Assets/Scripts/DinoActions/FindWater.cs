using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWater : GAction
{
    public override bool PrePerform()
    {
        FindClosestWaterIntersection();
        return true;
    }
    public override bool PostPerform()
    {
          
        return true;
    }

    private void FindClosestWaterIntersection()
    {
        if (meshGenerator == null)
        {
            Debug.LogError("MeshGenerator not found!");
            return;
        }

        Vector3[] vertices = meshGenerator.GetVertices();
        float waterHeight = meshGenerator.waterHeight;
        Vector3 closestPoint = Vector3.positiveInfinity;
        float closestDistance = Mathf.Infinity;

        //Debug.Log($"Water Height: {waterHeight}");

        foreach (Vector3 vertex in vertices)
        {
            // Determine if the vertex is close to the water height
            if (Mathf.Abs(vertex.y*meshGenerator.MESH_SCALE - waterHeight) < 2f) // Threshold can be adjusted
            {
                //Debug.Log($"Vertex near water height: {vertex}");

                // Calculate the distance from the AI agent
                float distance = Vector3.Distance(transform.position, vertex*meshGenerator.MESH_SCALE);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = vertex*meshGenerator.MESH_SCALE;
                    //Debug.Log($"New closest point: {closestPoint}, Distance: {closestDistance}");
                }
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
