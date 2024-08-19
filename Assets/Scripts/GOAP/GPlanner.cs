using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node parent; // Parent node in the graph
    public float cost; // Cost to reach this node
    public Dictionary<string, int> state; // State representation as a dictionary
    public GAction action; // Action associated with this node

    public Node(Node parent, float cost, Dictionary<string, int> allstates, GAction action)
    {
        this.parent = parent; // Initialize parent node
        this.cost = cost; // Initialize cost
        this.state = new Dictionary<string, int>(allstates); // Initialize state with a copy of allstates
        this.action = action; // Initialize action
    }
}

public class GPlanner
{
    // Plans a sequence of actions to achieve the given goal based on the current world states.
    public Queue<GAction> plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates states)
    {
        // Filter out actions that are not achievable.
        List<GAction> usableActions = new List<GAction>();
        foreach (GAction a in actions)
        {
            if (a.IsAchievable())
                usableActions.Add(a);
        }

        // List to store the possible paths (leaves) in the action graph.
        List<Node> leaves = new List<Node>();
        // Create the starting node with the initial world states.
        Node start = new Node(null, 0, GWorld.Instance.GetWorld().GetStates(), null);

        // Build the graph of actions to find possible paths to the goal.
        bool success = BuildGraph(start, leaves, usableActions, goal);
        if (!success)
        {
            Debug.Log("NO PLAN");
            return null;
        }

        // Find the cheapest path (least cost) among the leaves.
        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }
        }

        // Backtrack from the cheapest leaf to the start to get the sequence of actions.
        List<GAction> result = new List<GAction>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }

            n = n.parent;
        }

        // Convert the list of actions to a queue.
        Queue<GAction> queue = new Queue<GAction>();
        foreach (GAction a in result)
        {
            queue.Enqueue(a);
        }

        // Debugging: Print the plan.
        // Debug.Log("The Plan is: ");
        // foreach (GAction a in queue)
        // {
        //     Debug.Log("Q: " + a.actionName);
        // }

        return queue;
    }

    // Recursively builds the graph of actions to find paths to the goal.
    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions, Dictionary<string, int> goal)
    {
        bool foundPath = false;
        foreach (GAction action in usableActions)
        {
            if (action.IsAchievableGiven(parent.state))
            {
                // Create a new state based on the current state and the action's effects.
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                foreach (KeyValuePair<string, int> eff in action.effects)
                {
                    if (!currentState.ContainsKey(eff.Key))
                    {
                        currentState.Add(eff.Key, eff.Value);
                    }
                }

                // Create a new node with the updated state and action.
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);
                if (GoalAchieved(goal, currentState))
                {
                    // If the goal is achieved, add the node to the leaves.
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    // Recursively build the graph with the remaining actions.
                    List<GAction> subset = ActionSubSet(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                    {
                        foundPath = true;
                    }
                }
            }
        }
        return foundPath;
    }

    // Checks if the current state meets the goal requirements.
    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        foreach (KeyValuePair<string, int> g in goal)
        {
            if (!state.ContainsKey(g.Key))
            {
                return false;
            }
        }

        return true;
    }

    // Returns a subset of actions excluding the specified action.
    private List<GAction> ActionSubSet(List<GAction> actions, GAction removeMe)
    {
        List<GAction> subset = new List<GAction>();
        foreach (GAction a in actions)
        {
            if (!a.Equals(removeMe))
            {
                subset.Add(a);
            }
        }
        return subset;
    }
}