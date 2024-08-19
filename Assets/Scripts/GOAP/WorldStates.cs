using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public string key;
    public int value;
}

public class WorldStates : MonoBehaviour
{
    // Dictionary to store the states with their corresponding values
    public Dictionary<string, int> states;

    // Constructor to initialize the states dictionary
    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    // Method to check if a state exists in the dictionary
    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    // Method to add a new state to the dictionary
    public void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    // Method to modify the value of an existing state
    public void ModifyState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            states[key] += value;
            // If the state's value is less than or equal to 0, remove the state
            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else
        {
            // If the state does not exist, add it to the dictionary
            states.Add(key, value);
        }
    }

    // Method to remove a state from the dictionary
    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
        {
            states.Remove(key);
        }
    }

    // Method to set the value of a state, adding it if it does not exist
    public void SetState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            states[key] = value;
        }
        else
        {
            states.Add(key, value);
        }
    }

    // Method to get the dictionary of states
    public Dictionary<string, int> GetStates()
    {
        return states;
    }
}
