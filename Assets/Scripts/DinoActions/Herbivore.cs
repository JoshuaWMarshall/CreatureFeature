using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbivore : GAgent
{
    protected override void Start()
    {
        base.Start();
        RandomiseStats();
        worldStates.SetState("isThirsty", (int)thirst);
        worldStates.SetState("isHungry", (int)hunger);
        worldStates.SetState("isTired", 100 - (int)energy);

        InvokeRepeating("UpdateStates", 1f,1f);
    }

    void Update()
    {
        hunger = Mathf.Clamp(hunger + hungerRate * Time.deltaTime, 0, 100);
        energy = Mathf.Clamp(energy - energyRate * Time.deltaTime, 0, 100);
        thirst = Mathf.Clamp(thirst + thirstRate * Time.deltaTime, 0, 100);
    }

    private void RandomiseStats()
    {
        hunger = Random.Range(0, 30);
        thirst = Random.Range(0, 30);
        energy = Random.Range(70, 100);

        hungerRate = Random.Range(0.5f, 1.5f);
        energyRate = Random.Range(0.5f, 1.5f);
        thirstRate = Random.Range(0.5f, 1.5f);

    }

    private void UpdateStates()
    {
        worldStates.SetState("isThirsty", (int)thirst);
        worldStates.SetState("isHungry", (int)hunger);
        worldStates.SetState("isTired", 100 - (int)energy);
    }
}
