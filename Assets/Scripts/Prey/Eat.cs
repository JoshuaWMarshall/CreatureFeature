using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : GAction
{
    public float hungerModifier = 1;
    private float detectionRadius = 5f;
    private GameObject closestFood;
    
    public override bool PrePerform()
    {
         // FindClosestFood();
         // if (closestFood != null)
         // {
         //     Debug.Log("Found closest plant: " + closestFood.name);
         //     closestFood.GetComponent<RegrowPlant>()?.Eat();
         // }

         gAgent.hunger -= 10 * hungerModifier;

         gAgent.CompleteAction();
         return true;
    }
    public override bool PostPerform()
    {
        
        Debug.Log("Finished Eating");
        return true;
    }

    // private void FindClosestFood()
    // {
    //     Collider[] colliders = Physics.OverlapBox(gAgent.transform.position, new Vector3(detectionRadius, detectionRadius, detectionRadius), Quaternion.identity);
    //     closestFood = null;
    //     float closestDistance = Mathf.Infinity;
    //
    //     foreach (Collider collider in colliders)
    //     {
    //         if (collider.CompareTag("Plant"))
    //         {
    //             float distance = Vector3.Distance(transform.position, collider.transform.position);
    //             if (distance < closestDistance)
    //             {
    //                 closestDistance = distance;
    //                 closestFood = collider.gameObject;
    //             }
    //         }
    //     }
    // }
    
}
