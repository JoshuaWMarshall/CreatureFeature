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
    }

    public float hunger = 0;
    private bool goalAdded = false;

    private SubGoal s1;

    protected override void Start()
    {
        base.Start();
        SubGoal s2 = new SubGoal("Wait", 1, true);
        goals.Add(s2, 1);
    }

    void Update()
    {
        if (hunger > 0.5 && goalAdded == false)
        {
            s1 = new SubGoal("Eat", 1, true);
            goals.Add(s1, 3);
            goalAdded = true;
        }
        else if (hunger < 0.5 && goalAdded == true)
        {
            goals.Remove(s1);
            goalAdded = false;
        }

        if (hunger <= 1 && hunger >=0)
        {
            hunger += 0.01f * Time.deltaTime;
            Mathf.Clamp(hunger, 0, 1);
        }
    }
}
