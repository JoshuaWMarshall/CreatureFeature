using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Herbivore : GAgent
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("isWaiting", 1, true);
        goals.Add(s1, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
