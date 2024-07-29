using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : GAction
{
    private bool running = false;
    public override bool PrePerform()
    {
        return true;
    }
    public override bool PostPerform()
    {
        return true;
    }

    public void Update()
    {
        if (Herbivore.Instance.currentAction == this && running == false)
        {
            StartCoroutine(EffectHunger());
            running = true;
        }
        
        
    }

    IEnumerator EffectHunger()
    {
        yield return new WaitForSeconds(3);
        Herbivore.Instance.hunger -= 0.5f;
        running = false;
        yield return null;
    }
}
