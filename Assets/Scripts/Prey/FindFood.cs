using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindFood : GAction
{
   private float detectionRadius = 50f;
   private GameObject closestFood;
   
   public override bool PrePerform()
   {
      // FindClosestFood();
      // if (target != null)
      // {
      //    target = closestFood;
      // }

      return true;
   }
   public override bool PostPerform()
   {
      target = null;
      return true;
   }
   
   private void FindClosestFood()
   {
      Collider[] colliders = Physics.OverlapBox(gAgent.transform.position, new Vector3(detectionRadius, detectionRadius, detectionRadius), Quaternion.identity);
      closestFood = null;
      float closestDistance = Mathf.Infinity;

      foreach (Collider collider in colliders)
      {
         if (collider.CompareTag("Plant"))
         {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < closestDistance)
            {
               closestDistance = distance;
               closestFood = collider.gameObject;
            }
         }
      }
   }
   
}
