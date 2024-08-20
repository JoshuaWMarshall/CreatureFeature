using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // key: food game object, value: is it currently available
    public Dictionary<GameObject, bool> herbivoreFood;
    public Dictionary<GameObject, bool> carnivoreFood;
    
    // Method to initialize the dictionaries
    public void InitializeHerbivoreFoodDict(Dictionary<GameObject, bool> herbivoreFoodDict)
    {
        herbivoreFood = herbivoreFoodDict;
        Debug.Log("Herbivore Food Targets Initialized:");
    }
    // Method to initialize the dictionaries
    public void InitializeCarnivoreFoodDict(Dictionary<GameObject, bool> carnivoreFoodDict)
    {
        carnivoreFood = carnivoreFoodDict;
        Debug.Log("Carnivore Food Targets Initialized:");
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
