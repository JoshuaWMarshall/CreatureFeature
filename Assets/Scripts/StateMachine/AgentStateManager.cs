using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public enum AgentGoal
{
    Idle,
    Eating,
    Resting,
    Drinking
}

public class AgentStateManager : MonoBehaviour
{
    public Herbivore herbivore;
    //public WorldStates worldStates;

    private AgentBaseState currentState;
    public AgentGoal currentAgentGoal;
    public NavMeshAgent navMeshAgent;
    
    public IdleState idleState = new IdleState();
    public EatingState eatingState = new EatingState();
    public RestingState restingState = new RestingState();
    public DrinkingState drinkingState = new DrinkingState();

    void Start()
    {
        herbivore = GetComponent<Herbivore>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SwitchState(idleState); // Start in the Idle state
        
        InvokeRepeating("CheckTransition", 1f, 1f);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }

    void CheckTransition()
    {
        var states = herbivore.worldStates.GetStates();
        
        KeyValuePair<string, int> highestState = new KeyValuePair<string, int>("", -1);
        KeyValuePair<string, int> prevHighestState = new KeyValuePair<string, int>("", -1);

        foreach (var state in states)
        {
            if (state.Value > highestState.Value)
            {
                prevHighestState = highestState;
                highestState = state;
            }
        }

        if (highestState.Value <= 20)
        {
            SwitchState(idleState);
        }
        else if (highestState.Key == "isHungry")
        {
            SwitchState(eatingState);
        }
        else if (highestState.Key == "isTired")
        {
            SwitchState(restingState);
        }
        else if (highestState.Key == "isThirsty")
        {
            SwitchState(drinkingState);
        }
    }
    
    public void SwitchState(AgentBaseState newState)
    {
        if (newState == currentState)
        {
            return;
        }
        
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        
        currentState = newState;
        currentState.EnterState(this);
    }
}

#region States
public abstract class AgentBaseState
{
    protected SubGoal goal;
    
    public abstract void EnterState(AgentStateManager stateManager);
    public abstract void UpdateState(AgentStateManager stateManager);
    public abstract void ExitState(AgentStateManager stateManager);
}

public class IdleState : AgentBaseState
{
    private float wanderRadius = 10f;
    private float wanderTimer = 5f;
    private float timer;
    
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Idle State");
        stateManager.currentAgentGoal = AgentGoal.Idle;
        
        goal = new SubGoal("Idle", 1, false);
        
        stateManager.herbivore.goals.Add(goal, 1);
        
        timer = wanderTimer; // Start the timer
    }

    public override void UpdateState(AgentStateManager stateManager)
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(stateManager.transform.position, wanderRadius, 13);
            stateManager.navMeshAgent.SetDestination(newPos);
            timer = 0;
        }
    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Idle State");

        goal = stateManager.herbivore.GetGoal("Idle");
               
        stateManager.herbivore.goals.Remove(goal);
    }
    
    private Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}

public class EatingState : AgentBaseState
{
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Eating State");
        stateManager.currentAgentGoal = AgentGoal.Eating;

        goal = new SubGoal("Eat", 1, false);
        
        stateManager.herbivore.goals.Add(goal, 1);
    }

    public override void UpdateState(AgentStateManager stateManager)
    {
        
    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Eating State");

        goal = stateManager.herbivore.GetGoal("Eat");

        stateManager.herbivore.goals.Remove(goal);
    }
}

public class RestingState : AgentBaseState
{
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Resting State");
        stateManager.currentAgentGoal = AgentGoal.Resting;
        
        goal = new SubGoal("Rest", 1, false);
        stateManager.herbivore.goals.Add(goal, 1);
    }

    public override void UpdateState(AgentStateManager stateManager)
    {

    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Resting State");
        goal = stateManager.herbivore.GetGoal("Rest");
        stateManager.herbivore.goals.Remove(goal);
    }
}

public class DrinkingState : AgentBaseState
{
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Drinking State");
        stateManager.currentAgentGoal = AgentGoal.Drinking;
        
        goal = new SubGoal("Drink", 1, false);
        stateManager.herbivore.goals.Add(goal, 1);
    }

    public override void UpdateState(AgentStateManager stateManager)
    {
        
    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Drinking State");
        goal = stateManager.herbivore.GetGoal("Drink");
        stateManager.herbivore.goals.Remove(goal);
    }
}
#endregion

