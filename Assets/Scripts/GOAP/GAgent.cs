using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal
{
    public Dictionary<string, int> sgoals;
    public bool remove;

    public SubGoal(string subGoal, int i, bool remove)
    {
        sgoals = new Dictionary<string, int>();
        sgoals.Add(subGoal, i);
        this.remove = remove;
    }
}
public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    public WorldStates worldStates;

    GPlanner planner;
    public Queue<GAction> actionQueue;
    public GAction currentAction;
    SubGoal currentGoal;

    public int sightRadius = 200;

    public float hunger = 0;
    public float energy = 100;
    public float thirst = 0;

    public float hungerRate = 1f;
    public float energyRate = 1f;
    public float thirstRate = 1f;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //worldStates = gameObject.AddComponent<WorldStates>();

        GAction[] acts = this.GetComponents<GAction>();
        foreach (GAction a in acts)
        {
            actions.Add(a);
            a.gAgent = this;
        }
    }

    private bool invoked = false;

    public GAction GetActionByName(string actionName)
    {
        return actions.FirstOrDefault(a => a.actionName == actionName);
    }

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

    public void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false; 
    }

     void LateUpdate()
    {
        if (currentAction != null && currentAction.running)
        {
            float distanceToTarget =
                Vector3.Distance(currentAction.target.transform.position, this.transform.position);
            if (currentAction.npc.Path.Count > 0 && distanceToTarget < 5f)
            {
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }


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

        if (actionQueue != null && actionQueue.Count == 0)
        {
            if (currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }

            planner = null;
        }

        if (actionQueue != null && actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

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
