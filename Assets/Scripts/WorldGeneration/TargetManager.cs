using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // key: food game object, value: is it currently available
    public Dictionary<GameObject, bool> herbivoreFood;
    public Dictionary<GameObject, bool> carnivoreFood;

    // Method to initialize the dictionaries
    public void Initialize(Dictionary<GameObject, bool> herbivoreFoodDict, Dictionary<GameObject, bool> carnivoreFoodDict)
    {
        herbivoreFood = herbivoreFoodDict;
        carnivoreFood = carnivoreFoodDict;
    }
    
    
    public void ReleaseHerbivoreTarget(GameObject target)
    {
        if (herbivoreFood.ContainsKey(target))
        {
            herbivoreFood[target] = true; // Mark as available again
        }
    }
    
    public void ReleaseCarnivoreTarget(GameObject target)
    {
        if (carnivoreFood.ContainsKey(target))
        {
            carnivoreFood[target] = true; // Mark as available again
        }
    }

    public void RemoveHerbivoreTarget(GameObject target)
    {
        if (herbivoreFood.ContainsKey(target))
        {
            herbivoreFood.Remove(target);
            Destroy(target);
        }
    }
    
    public void RemoveCarnivoreTarget(GameObject target)
    {
        if (carnivoreFood.ContainsKey(target))
        {
            carnivoreFood.Remove(target);
            Destroy(target);
        }
    }
}
