using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : GAction
{
    public float hungerModifier = 1;
    public override bool PrePerform()
    {
        return true;
    }
    public override bool PostPerform()
    {
        gAgent.hunger -= 10 * hungerModifier;

        if (CompareTag("Herbivore"))
        {
            targetManager.RemoveHerbivoreTarget(target);
        }
        else
        {
            targetManager.RemoveCarnivoreTarget(target);
        }
        
        gAgent.currentAction.target = null;

        Debug.Log("Finished Eating");
        return true;
    }
}
