using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    // Dictionary to store herbivore food targets and their availability status
    public Dictionary<GameObject, bool> herbivoreFood;
    
    // Dictionary to store carnivore food targets and their availability status
    public Dictionary<GameObject, bool> carnivoreFood;

    // Method to initialize the herbivore food dictionary
    public void InitializeHerbivoreFoodDict(Dictionary<GameObject, bool> herbivoreFoodDict)
    {
        herbivoreFood = herbivoreFoodDict;
        Debug.Log("Herbivore Food Targets Initialized:");
    }

    // Method to initialize the carnivore food dictionary
    public void InitializeCarnivoreFoodDict(Dictionary<GameObject, bool> carnivoreFoodDict)
    {
        carnivoreFood = carnivoreFoodDict;
        Debug.Log("Carnivore Food Targets Initialized:");
    }
    
    // Method to mark a herbivore food target as available
    public void ReleaseHerbivoreTarget(GameObject target)
    {
        if (herbivoreFood.ContainsKey(target))
        {
            herbivoreFood[target] = true; // Mark as available again
        }
    }
    
    // Method to mark a carnivore food target as available
    public void ReleaseCarnivoreTarget(GameObject target)
    {
        if (carnivoreFood.ContainsKey(target))
        {
            carnivoreFood[target] = true; // Mark as available again
        }
    }

    // Method to remove a herbivore food target and destroy the game object
    public void RemoveHerbivoreTarget(GameObject target)
    {
        if (herbivoreFood.ContainsKey(target))
        {
            herbivoreFood.Remove(target);
            Destroy(target);
        }
    }
    
    // Method to remove a carnivore food target and destroy the game object
    public void RemoveCarnivoreTarget(GameObject target)
    {
        if (carnivoreFood.ContainsKey(target))
        {
            carnivoreFood.Remove(target);
            Destroy(target);
        }
    }
}
