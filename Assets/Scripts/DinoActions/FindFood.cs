using UnityEngine;

public class FindFood : GAction
{
    private Eat eat;

    public override void Start()
    {
        base.Start();
        eat = GetComponent<Eat>(); // Get the Eat component attached to the same GameObject
    }

    // Called before the action is performed
    public override bool PrePerform()
    {
        return FindClosestFood(); // Attempt to find the closest food
    }

    // Called after the action is performed
    public override bool PostPerform()
    {
        return true; // Indicate that the post-perform action was successful
    }

    // Method to find the closest food based on the type of the dinosaur (Herbivore or Carnivore)
    private bool FindClosestFood()
    {
        if (CompareTag("Herbivore"))
        {
            // Find all colliders within the sight radius that are on the herbivore food layer (layer 14: Trees)
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, gAgent.sightRadius, 1 << 14);
            Collider closestTarget = null;
            float closestDistance = Mathf.Infinity;
            
            // Iterate through all found colliders to find the closest valid food target
            foreach (Collider hitCollider in hitColliders)
            {
                if (!hitCollider.CompareTag(targetTag)) continue;
                var distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                // Check if the food is available and closer than the previously found food
                if (!(distance < closestDistance) ||
                    !targetManager.herbivoreFood.ContainsKey(hitCollider.gameObject) ||
                    !targetManager.herbivoreFood[hitCollider.gameObject]) continue;
                
                closestDistance = distance;
                closestTarget = hitCollider;
            }

            if (closestTarget == null) return false; // No valid target found
            
            this.target = closestTarget.gameObject; // Set the target to the closest food
            targetManager.herbivoreFood[target] = false; // Mark the food as unavailable
            this.eat.target = target; // Set the Eat component's target to the found food
            return true;
        }
        else if (CompareTag("Carnivore"))
        {
            // Find all colliders within the sight radius that are on the carnivore food layer (layer 15: Dinos)
            // Set QueryTriggerInteraction to Collide as the collider on dinos are a trigger
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, gAgent.sightRadius, 1 << 15, QueryTriggerInteraction.Collide);
            Collider closestTarget = null;
            float closestDistance = Mathf.Infinity;
            
            // Iterate through all found colliders to find the closest valid food target
            foreach (Collider hitCollider in hitColliders)
            {
                if (!hitCollider.CompareTag(targetTag)) continue;
                var distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                
                // Check if the food is available and closer than the previously found food
                if (!(distance < closestDistance) || 
                    !targetManager.carnivoreFood.ContainsKey(hitCollider.gameObject) ||
                    !targetManager.carnivoreFood[hitCollider.gameObject]) continue;
                
                closestDistance = distance;
                closestTarget = hitCollider;
            }

            if (closestTarget == null) return false; // No valid target found
            
            this.target = closestTarget.gameObject; // Set the target to the closest food
            targetManager.carnivoreFood[target] = false; // Mark the food as unavailable
            this.eat.target = target; // Set the Eat component's target to the found food
            return true;
        }

        return false; // No valid target found
    }
}