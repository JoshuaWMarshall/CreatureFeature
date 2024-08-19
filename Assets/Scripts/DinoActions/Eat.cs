using UnityEngine;

public class Eat : GAction
{
    // Modifier to adjust the amount of hunger reduced
    public float hungerModifier = 1;

    // This method is called before the action is performed
    public override bool PrePerform()
    {
        // Always return true to indicate the action can be performed
        return true;
    }

    // This method is called after the action is performed
    public override bool PostPerform()
    {
        // Reduce the agent's hunger by a value modified by hungerModifier
        gAgent.hunger -= 10 * hungerModifier;

        // Check if the agent is a herbivore or carnivore and remove the target accordingly
        if (CompareTag("Herbivore"))
        {
            targetManager.RemoveHerbivoreTarget(target);
        }
        else
        {
            targetManager.RemoveCarnivoreTarget(target);
        }
        
        // Clear the current action's target
        gAgent.currentAction.target = null;

        // Log a message indicating the eating action is finished
        Debug.Log("Finished Eating");
        
        // Always return true to indicate the post-perform actions were successful
        return true;
    }
}
