using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public abstract class GAction : MonoBehaviour
{
    public string actionName = "Action";     // The name of the action
    public float cost = 1.0f;     // The cost associated with performing the action
    public GameObject target;     // The target GameObject for the action
    public string targetTag; // The tag of the target GameObject
    public float duration = 0;  // The duration required to perform the action
    public WorldState[] preConditions; // The preconditions that must be met before the action can be performed
    public WorldState[] afterEffects; // The effects that occur after the action is performed
    public NPC npc;  // Reference to the NPC performing the action
    protected TerrainGenerationData terrainGenerationData; // Reference to the TerrainGenerationData used for terrain generation
    protected TerrainGeneration terrainGeneration; // Reference to the TerrainGeneration component
    public GAgent gAgent;  // Reference to the GAgent associated with this action
    public TargetManager targetManager; // Reference to the TargetManager component
    public Dictionary<string, int> preconditions; // Dictionary to store preconditions as key-value pairs
    public Dictionary<string, int> effects;  // Dictionary to store effects as key-value pairs
    public bool running = false; // Flag to indicate if the action is currently running;

    // Static property to generate a unique terrain save name based on the product name and active scene
    private static string terrainSaveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TerrainData"; }
    }
    
    // Virtual method called when the script instance is being loaded
    public virtual void Start()
    {
        // Initialize references to NPC, TargetManager, TerrainGenerationData, and TerrainGeneration
        npc = GetComponent<NPC>();
        targetManager = FindObjectOfType<TargetManager>();
        terrainGenerationData = TerrainGenerationData.Load(terrainSaveName);
        terrainGeneration = FindObjectOfType<TerrainGeneration>();
    }

    // Constructor to initialize preconditions and effects dictionaries
    public GAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    // Method called when the script instance is being loaded
    public void Awake()
    {
        // Populate preconditions dictionary from preConditions array
        if (preConditions != null)
        {
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }
        }
        
        // Populate effects dictionary from afterEffects array
        if (afterEffects != null)
        {
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key, w.value);
            }
        }
    }

    // Method to check if the action is achievable
    public bool IsAchievable()
    {
        return true;
    }

    // Method to check if the action is achievable given a set of conditions
    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> p in preconditions)
        {
            if (!conditions.ContainsKey(p.Key))
            {
                return false;
            }
        }

        return true;
    }

    // Abstract method to define pre-performance behavior
    public abstract bool PrePerform();
    
    // Abstract method to define post-performance behavior
    public abstract bool PostPerform();
}