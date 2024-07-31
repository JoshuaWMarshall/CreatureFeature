using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkWater : GAction
{
   public float thirstModifier = 1;
   public override bool PrePerform()
   {
      gAgent.thirst -= 20 * thirstModifier;
      gAgent.CompleteAction();
      return true;
   }
   public override bool PostPerform()
   {
      
      return true;
   }

}
