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
    // Reference to the GAgent component attached to the GameObject
    public GAgent gAgent;
    
    // The current state of the agent
    private AgentBaseState currentState;
    
    // The current goal of the agent
    public AgentGoal currentAgentGoal;
    
    // Predefined states for the agent
    public IdleState idleState = new IdleState();
    public EatingState eatingState = new EatingState();
    public RestingState restingState = new RestingState();
    public DrinkingState drinkingState = new DrinkingState();
    
    // Dictionary to hold the states and their corresponding values
    public Dictionary<string, int> states;

    void Start()
    {
        // Initialize the gAgent component and retrieve its world states
        gAgent = GetComponent<GAgent>();
        states = gAgent.worldStates.GetStates();

        // Start in the Idle state
        SwitchState(idleState);
    }

    void Update()
    {
        // Update the current state of the agent
        currentState.UpdateState(this);
    }

    // Method to switch the agent's state
    public void SwitchState(AgentBaseState newState)
    {
        // If the new state is the same as the current state, do nothing
        if (newState == currentState)
        {
            return;
        }
        
        // Exit the current state if it exists
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        
        // Enter the new state
        currentState = newState;
        currentState.EnterState(this);
    }

    // Overloaded method to switch the agent's state based on the goal
    public void SwitchState(AgentGoal newState)
    {
        // If the new goal is the same as the current goal, do nothing
        if (newState == currentAgentGoal)
        {
            return;
        }

        // Exit the current state if it exists
        if(currentState != null)
        {
            currentState.ExitState(this);
        }

        // Switch to the appropriate state based on the new goal
        switch (newState)
        {
            case AgentGoal.Idle:
                currentState = idleState;
                break;
            case AgentGoal.Eating:
                currentState = eatingState;
                break;
            case AgentGoal.Drinking:
                currentState = drinkingState;
                break;
            case AgentGoal.Resting:
                currentState = restingState;
                break;
        }

        // Enter the new state
        currentState.EnterState(this);
    }
}

#region states
public abstract class AgentBaseState
{
    // The goal associated with this state
    protected SubGoal goal;
    
    // Method to handle actions when entering the state
    public abstract void EnterState(AgentStateManager stateManager);
    
    // Method to handle actions during the state update
    public abstract void UpdateState(AgentStateManager stateManager);
    
    // Method to handle actions when exiting the state
    public abstract void ExitState(AgentStateManager stateManager);
}

public class IdleState : AgentBaseState
{
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Idle State");
        stateManager.currentAgentGoal = AgentGoal.Idle;

        // Clear current goals
        stateManager.gAgent.goals.Clear();

        // Set the goal for the Idle state
        goal = new SubGoal("Idle", 1, false);

        // Add the goal to the agent's goals
        stateManager.gAgent.goals.Add(goal, 1);
    }

    public override void UpdateState(AgentStateManager stateManager)
    {
        // State transitions based on agent's needs
        if (stateManager.states["isHungry"] >= 50)
        {
            stateManager.SwitchState(stateManager.eatingState);
        }
        else if (stateManager.states["isTired"] >= 50)
        {
            stateManager.SwitchState(stateManager.restingState);
        }
        else if (stateManager.states["isThirsty"] >= 50)
        {
            stateManager.SwitchState(stateManager.drinkingState);
        }
    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Idle State");

        // Retrieve and remove the Idle goal from the agent's goals
        goal = stateManager.gAgent.GetGoal("Idle");

        if (goal != null)
        {
            stateManager.gAgent.goals.Remove(goal);
        }
    }
}

public class EatingState : AgentBaseState
{
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Eating State");
        stateManager.currentAgentGoal = AgentGoal.Eating;

        // Set the goal for the Eating state
        goal = new SubGoal("Eat", 1, true);
        
        // Add the goal to the agent's goals
        stateManager.gAgent.goals.Add(goal, 1);
    }

    public override void UpdateState(AgentStateManager stateManager)
    {
        // Transition back to Idle state if hunger is below threshold
        if(stateManager.states["isHungry"] < 30)
        {
            stateManager.SwitchState(stateManager.idleState);
        }
    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Eating State");

        // Retrieve and remove the Eat goal from the agent's goals
        goal = stateManager.gAgent.GetGoal("Eat");

        if (goal != null)
        {
            stateManager.gAgent.goals.Remove(goal);
        }
    }
}

public class RestingState : AgentBaseState
{
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Resting State");
        stateManager.currentAgentGoal = AgentGoal.Resting;
        
        // Set the goal for the Resting state
        goal = new SubGoal("Rest", 1, true);
        
        // Add the goal to the agent's goals
        stateManager.gAgent.goals.Add(goal, 1);
    }

    public override void UpdateState(AgentStateManager stateManager)
    {
        // Transition back to Idle state if tiredness is below threshold
        if (stateManager.states["isTired"] < 30)
        {
            stateManager.SwitchState(stateManager.idleState);
        }
    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Resting State");

        // Retrieve and remove the Rest goal from the agent's goals
        goal = stateManager.gAgent.GetGoal("Rest");
        
        if (goal != null)
        {
            stateManager.gAgent.goals.Remove(goal);
        }
    }
}

public class DrinkingState : AgentBaseState
{
    public override void EnterState(AgentStateManager stateManager)
    {
        Debug.Log("Entering Drinking State");
        stateManager.currentAgentGoal = AgentGoal.Drinking;
        
        // Set the goal for the Drinking state
        goal = new SubGoal("Drink", 1, true);
        
        // Add the goal to the agent's goals
        stateManager.gAgent.goals.Add(goal, 1);
    }

    public override void UpdateState(AgentStateManager stateManager)
    {
        // Transition back to Idle state if thirst is below threshold
        if (stateManager.states["isThirsty"] < 30)
        {
            stateManager.SwitchState(stateManager.idleState);
        }
    }

    public override void ExitState(AgentStateManager stateManager)
    {
        Debug.Log("Exiting Drinking State");

        // Retrieve and remove the Drink goal from the agent's goals
        goal = stateManager.gAgent.GetGoal("Drink");
        
        if (goal != null)
        {
            stateManager.gAgent.goals.Remove(goal);
        }
    }
}
#endregion

