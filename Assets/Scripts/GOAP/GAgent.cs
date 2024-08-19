using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal
{
    public Dictionary<string, int> sgoals;     // Dictionary to store sub-goals with their respective priority levels
    public bool remove;  // Flag to indicate whether the sub-goal should be removed after completion

    // Constructor to initialize the sub-goal with a name, priority, and removal flag
    public SubGoal(string subGoal, int i, bool remove)
    {
        // Initialize the dictionary and add the provided sub-goal with its priority
        sgoals = new Dictionary<string, int>();
        sgoals.Add(subGoal, i);
        
        // Set the removal flag
        this.remove = remove;
    }
}
public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>(); // List of actions available to the agent
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>(); // Dictionary of goals with their priority levels
    public WorldStates worldStates;  // World states known to the agent
    GPlanner planner;  // Planner for determining the sequence of actions to achieve goals
    public Queue<GAction> actionQueue; // Queue of actions to be performed
    public GAction currentAction; // Currently executing action
    SubGoal currentGoal; // Currently pursued goal
    public int sightRadius = 200;  // Agent's sight radius

    // Agent's needs
    public float hunger = 0;
    public float energy = 100;
    public float thirst = 0;

    // Rates at which the agent's needs change
    public float hungerRate = 1f;
    public float energyRate = 1f;
    public float thirstRate = 1f;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Initialize world states
        worldStates = gameObject.AddComponent<WorldStates>();

        // Get all actions attached to the agent and add them to the actions list
        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts)
        {
            actions.Add(a);
            a.gAgent = this;
        }
    }

    // Flag to check if the action completion has been invoked
    private bool invoked = false;

    // Get an action by its name
    public GAction GetActionByName(string actionName)
    {
        return actions.FirstOrDefault(a => a.actionName == actionName);
    }

    // Get a goal by its name
    public SubGoal GetGoal(string goalName)
    {
        SubGoal goalToGet = null;

        foreach (KeyValuePair<SubGoal, int> g in goals)
        {
            if (g.Key.sgoals.ContainsKey(goalName))
            {
                goalToGet = g.Key;
            }
        }

        return goalToGet;
    }

    // Complete the current action
    public void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false; 
    }

    // LateUpdate is called once per frame after all Update functions have been called
    void LateUpdate()
    {
        // If there is a current action and it is running
        if (currentAction != null && currentAction.running)
        {
            // Calculate the distance to the target
            float distanceToTarget = Vector3.Distance(currentAction.target.transform.position, this.transform.position);
            
            // If the NPC has a path and is close to the target
            if (currentAction.npc.Path.Count > 0 && distanceToTarget < 5f)
            {
                // Invoke the completion of the action if not already invoked
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        // If there is no planner or action queue, create a new plan
        if (planner == null || actionQueue == null)
        {
            planner = new GPlanner();
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                actionQueue = planner.plan(actions, sg.Key.sgoals, null);
                if (actionQueue != null)
                {
                    currentGoal = sg.Key;
                    break;
                }
            }
        }

        // If the action queue is empty, remove the current goal if necessary and reset the planner
        if (actionQueue != null && actionQueue.Count == 0)
        {
            if (currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }

            planner = null;
        }

        // If there are actions in the queue, dequeue the next action and perform it
        if (actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if (currentAction.PrePerform())
            {
                // Find the target if it is not already set
                if (currentAction.target == null && currentAction.targetTag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                // If the target is found, set the action to running and set the NPC's destination
                if (currentAction.target != null)
                {
                    currentAction.running = true;
                    currentAction.npc.SetDestination(currentAction.target.transform);
                }
            }
            else
            {
                actionQueue = null;
            }
        }
    }
}