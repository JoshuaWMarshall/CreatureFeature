using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resting : GAction
{
    private float detectionRadius = 500f;
   public float restModifier = 1;
   
   public override bool PrePerform()
   {
        //FindClosestRestArea();
       return true;
   }
   public override bool PostPerform()
   {
        gAgent.energy += 10 * restModifier;
        return true;
   }
   
   private void FindClosestRestArea()
   {
       Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, detectionRadius);
       Collider closestTarget = null;
       float closestDistance = Mathf.Infinity;

       foreach (Collider hitCollider in hitColliders)
       {
           if (hitCollider.CompareTag(this.targetTag))
           {
               Debug.Log("Found target: " + hitCollider.name);

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
