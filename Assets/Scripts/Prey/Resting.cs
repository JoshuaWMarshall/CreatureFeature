using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resting : GAction
{
   public float restModifier = 1;
   
   public override bool PrePerform()
   {
      
      return true;
   }
   public override bool PostPerform()
   {
        gAgent.energy += 10 * restModifier;
        return true;
   }
   
}
