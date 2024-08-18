using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public abstract class GAction : MonoBehaviour
{
    public string actionName = "Action";
    public float cost = 1.0f;
    public GameObject target;
    public string targetTag;
    public float duration = 0;
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NPC npc;
    //public MeshGenerator meshGenerator;
    protected TerrainGenerationData terrainGenerationData;
    protected TerrainGeneration terrainGeneration;
    public GAgent gAgent;
    public TargetManager targetManager;
    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effects;
    
    public bool running = false;

    private static string terrainSaveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TerrainData"; }
    }
    
    public virtual void Start()
    {
        npc = GetComponent<NPC>();
        targetManager = FindObjectOfType<TargetManager>();
        terrainGenerationData = TerrainGenerationData.Load(terrainSaveName);
        terrainGeneration = FindObjectOfType<TerrainGeneration>();
    }

    public GAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    public void Awake()
    {
        if (preConditions != null)
        {
            foreach (WorldState w in preConditions)
            {
                preconditions.Add(w.key,w.value);
            }
        }
        if (afterEffects != null)
        {
            foreach (WorldState w in afterEffects)
            {
                effects.Add(w.key,w.value);
            }
        }
    }

    public bool IsAchievable()
    {
        return true;
    }

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

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}
