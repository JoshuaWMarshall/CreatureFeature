using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FindFood : GAction
{
    private Eat eat;
    public override void Start()
    {
        base.Start();
        eat = GetComponent<Eat>();
    }
    public override bool PrePerform()
    {
        return  FindClosestFood();
    }
    public override bool PostPerform()
    {
        return true;
    }

    private bool FindClosestFood()
    {
        if (CompareTag("Herbivore"))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, gAgent.sightRadius, 1 << 14);
            Collider closestTarget = null;
            float closestDistance = Mathf.Infinity;
            
            foreach (Collider hitCollider in hitColliders)
            {
                if (!hitCollider.CompareTag(targetTag)) continue;
                var distance = Vector3.Distance(transform.position, hitCollider.transform.position);

                if (!(distance < closestDistance) ||
                    !targetManager.herbivoreFood.ContainsKey(hitCollider.gameObject) ||
                    !targetManager.herbivoreFood[hitCollider.gameObject]) continue;
                
                closestDistance = distance;
                closestTarget = hitCollider;
            }

            if (closestTarget == null) return false; // No valid target found
            
            this.target = closestTarget.gameObject;
            targetManager.herbivoreFood[target] = false; // Mark as unavailable
            this.eat.target = target;
            return true;
        }
        else if (CompareTag("Carnivore"))
        {
            // set QueryTriggerInteraction to Collide as the collider on dinos are a trigger
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, gAgent.sightRadius, 1 << 15, QueryTriggerInteraction.Collide);
            Collider closestTarget = null;
            float closestDistance = Mathf.Infinity;
            
            // Find the closest carnivore food directly
            foreach (Collider hitCollider in hitColliders)
            {
                if (!hitCollider.CompareTag(targetTag)) continue;
                var distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                
                if (!(distance < closestDistance) || 
                    !targetManager.carnivoreFood.ContainsKey(hitCollider.gameObject) ||
                    !targetManager.carnivoreFood[hitCollider.gameObject]) continue;
                
                closestDistance = distance;
                closestTarget = hitCollider;
            }

            if (closestTarget == null) return false; // No valid target found
            
            this.target = closestTarget.gameObject;
            targetManager.carnivoreFood[target] = false;
            this.eat.target = target;
            return true;
        }

        return false; // No valid target found
    }
}
