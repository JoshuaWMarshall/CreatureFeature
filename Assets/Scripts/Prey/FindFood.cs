using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindFood : GAction
{
    private float detectionRadius = 500f;
    //private GameObject closestFood;

    public override bool PrePerform()
    {
        FindClosestFood();

        return true;
    }
    public override bool PostPerform()
    {
        return true;
    }

    private void FindClosestFood()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, detectionRadius);
        Collider closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(this.targetTag))
            {
                //Debug.Log("Found target: " + hitCollider.name);

                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider;
                }
            }
        }

        if (closestTarget != null)
        {
            this.target = closestTarget.gameObject;
        }
    }
}
