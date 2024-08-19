using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resting : GAction
{
    // Radius within which the resting area will be detected
    private float detectionRadius = 500f;
    
    // Modifier to adjust the amount of energy gained during rest
    public float restModifier = 1;
   
    // Method called before the action is performed
    public override bool PrePerform()
    {
        FindClosestRestArea();
        return true;
    }

    // Method called after the action is performed
    public override bool PostPerform()
    {
        // Increase the agent's energy by a value modified by the restModifier
        gAgent.energy += 10 * restModifier;
        return true;
    }
   
    // Method to find the closest rest area within the detection radius
    private void FindClosestRestArea()
    {
        // Get all colliders within the detection radius
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, detectionRadius);
        Collider closestTarget = null;
        float closestDistance = Mathf.Infinity;

        // Iterate through all detected colliders
        foreach (Collider hitCollider in hitColliders)
        {
            // Check if the collider has the target tag
            if (hitCollider.CompareTag(this.targetTag))
            {
                Debug.Log("Found target: " + hitCollider.name);

                // Calculate the distance to the collider
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                
                // Update the closest target if the current collider is closer
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider;
                }
            }
        }

        // Set the closest target as the current target if one was found
        if (closestTarget != null)
        {
            this.target = closestTarget.gameObject;
        }
    }
}