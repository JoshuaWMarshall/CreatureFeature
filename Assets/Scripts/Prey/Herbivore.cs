using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbivore : GAgent
{
    private static Herbivore _instance;
    public static Herbivore Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Herbivore();
            }
            return _instance;
        }
    }
    
    private Herbivore()
    {
        // Private constructor to prevent external instantiation
        hunger = 0;
        energy = 100;
        thirst = 0;
    }
    
    protected override void Start()
    {
        base.Start();
        
        InvokeRepeating("UpdateStates", 1f,1f);
    }

    void Update()
    {
        hunger = Mathf.Clamp(hunger + hungerRate * Time.deltaTime, 0, 100);
        energy = Mathf.Clamp(energy - energyRate * Time.deltaTime, 0, 100);
        thirst = Mathf.Clamp(thirst + thirstRate * Time.deltaTime, 0, 100);
    }

    private void UpdateStates()
    {
        worldStates.SetState("isThirsty", (int)thirst);
        worldStates.SetState("isHungry", (int)hunger);
        worldStates.SetState("isTired", 100 - (int)energy);
    }
}
