using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : GAction
{
    private bool running2 = false;
    public override bool PrePerform()
    {
        return true;
    }
    public override bool PostPerform()
    {
        return true;
    }

    /*public void Update()
    {
        if (Herbivore.Instance.currentAction == this && running2 == false)
        {
            Debug.Log("coretine start");
            StartCoroutine(EffectHunger());
            running2 = true;
        }
        
        
    }*/

    /*IEnumerator EffectHunger()
    {
        yield return new WaitForSeconds(3);
        Herbivore.Instance.hunger -= 0.5f;
        running2 = false;
        yield return null;
    }*/
}
