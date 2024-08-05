using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkWater : GAction
{
   public float thirstModifier = 1;
   public override bool PrePerform()
   {

      return true;
   }
   public override bool PostPerform()
   {
        gAgent.thirst -= 20 * thirstModifier;
        return true;
   }

}
