using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWater : GAction
{
    private MeshGenerator meshGenerator;

    public override void Start()
    {
        base.Start();
        meshGenerator = FindObjectOfType<MeshGenerator>();
    }

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

        foreach (Vector3 vertex in vertices)
        {
            if (Mathf.Abs(vertex.y - waterHeight) < 0.1f) // Adjust the threshold as needed
            {
                float distance = Vector3.Distance(transform.position, vertex);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPoint = vertex;
                }
            }
        }

        if (closestPoint != Vector3.positiveInfinity)
        {
            target = new GameObject("WaterTarget");

            target.transform.position = closestPoint;
        }
    }
   
}
