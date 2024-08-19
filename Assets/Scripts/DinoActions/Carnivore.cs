using UnityEngine;

public class Carnivore : GAgent
{
    protected override void Start()
    {
        base.Start(); // Call the base class Start method
        RandomiseStats(); // Initialize the stats with random values
        worldStates.SetState("isThirsty", (int)thirst); // Set initial thirst state
        worldStates.SetState("isHungry", (int)hunger); // Set initial hunger state
        worldStates.SetState("isTired", 100 - (int)energy); // Set initial energy state

        // Repeatedly call UpdateStates method every 1 second
        InvokeRepeating("UpdateStates", 1f, 1f);
    }
    
    void Update()
    {
        // Update hunger, energy, and thirst values over time
        hunger = Mathf.Clamp(hunger + hungerRate * Time.deltaTime, 0, 100);
        energy = Mathf.Clamp(energy - energyRate * Time.deltaTime, 0, 100);
        thirst = Mathf.Clamp(thirst + thirstRate * Time.deltaTime, 0, 100);
    }

    // Randomize the initial stats for hunger, thirst, and energy
    private void RandomiseStats()
    {
        hunger = Random.Range(0, 30);
        thirst = Random.Range(0, 30);
        energy = Random.Range(70, 100);

        hungerRate = Random.Range(0.5f, 1.5f);
        energyRate = Random.Range(0.5f, 1.5f);
        thirstRate = Random.Range(0.5f, 1.5f);
    }

    // Update the world states for thirst, hunger, and energy
    private void UpdateStates()
    {
        worldStates.SetState("isThirsty", (int)thirst);
        worldStates.SetState("isHungry", (int)hunger);
        worldStates.SetState("isTired", 100 - (int)energy);
    }
}
