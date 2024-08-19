using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TreePlacement : MonoBehaviour
{
    private TargetManager targetManager; // Reference to the TargetManager component
    private Dictionary<GameObject, bool> herbivoreFood = new Dictionary<GameObject, bool>();   // Dictionary to keep track of herbivore food sources

    // Containers for organizing tree and rest area GameObjects
    private GameObject restAreaContainer;
    private GameObject treeContainer;
    
    private void Start()
    {
        // Find and assign the TargetManager if not already assigned
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        // Initialize the herbivore food dictionary in the TargetManager
        targetManager.InitializeHerbivoreFoodDict(herbivoreFood);
    }

    // Method to place trees and rest areas based on the provided data
    public void PlaceTrees(TreePlacementData data, TerrainGeneration terrainGeneration,
        TerrainGenerationData terrainGenerationData, GameObject treePrefab, GameObject restAreaPrefab)
    {
        // Clear existing trees and rest areas
        ClearTrees(data);

        // Get the vertices of the terrain mesh
        Vector3[] vertices = terrainGeneration.mesh.vertices;
        
        // Iterate through each vertex to determine tree placement
        for (int i = 0; i < vertices.Length; i++)
        {
            // Check if the vertex is suitable for tree placement based on fitness
            if (Fitness(data.noiseMap, terrainGeneration.mesh, terrainGenerationData.meshScale, data.maxSteepness, terrainGenerationData.waterHeight, i) > 1 - data.intensity)
            {
                // Apply a random offset to the position
                Vector3 pos = new Vector3(vertices[i].x + Random.Range(-data.randomness, data.randomness),
                    vertices[i].y,  vertices[i].z + Random.Range(-data.randomness, data.randomness)) * terrainGenerationData.meshScale;
                
                // Create a new tree container if it doesn't exist
                if (treeContainer == null)
                {
                    treeContainer = new GameObject();
                    treeContainer.name = "Tree Container";
                    treeContainer.tag = "TreeContainer";
                }
                
                // Instantiate the tree prefab at the calculated position
                GameObject clone = Instantiate(treePrefab, pos, Quaternion.identity, treeContainer.transform);
                clone.name = $"Poplar Tree #{i}";
                herbivoreFood.Add(clone, true);

                // Randomly place rest areas near some trees
                if (Random.Range(0, 5) < 1)
                {
                    // Create a new rest area container if it doesn't exist
                    if (restAreaContainer == null)
                    {
                        restAreaContainer = new GameObject();
                        restAreaContainer.name = "Rest Area Container";
                        restAreaContainer.tag = "RestAreaContainer";
                    }
                    
                    // Instantiate the rest area prefab at a nearby position
                    GameObject restArea = Instantiate(restAreaPrefab, pos + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)),
                        Quaternion.identity, restAreaContainer.transform);
                }
            }
        }
        
        // Ensure the TargetManager is assigned
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
    }

    // Method to clear existing trees and rest areas
    public void ClearTrees(TreePlacementData data)
    {
        // Find and assign the rest area container if not already assigned
        if (restAreaContainer == null)
        {
            restAreaContainer = GameObject.FindGameObjectWithTag("RestAreaContainer");
        }

        // Find and assign the tree container if not already assigned
        if (treeContainer == null)
        {
            treeContainer = GameObject.FindGameObjectWithTag("TreeContainer");
        }
        
        // Destroy the tree and rest area containers immediately
        DestroyImmediate(treeContainer);
        DestroyImmediate(restAreaContainer);

        // Clear the herbivore food dictionary if it contains any entries
        if (herbivoreFood.Count > 0)
        {
            herbivoreFood.Clear();
        }
    }

    // Method to calculate the fitness of a vertex for tree placement
    private float Fitness(Texture2D noiseMap, Mesh mesh, float meshScale, float maxSteepness, float waterHeight, int index)
    {
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
                
        // Get the fitness value from the noise map
        float fitness = noiseMap.GetPixel((int)vertices[index].x, (int)vertices[index].z).g;
        
        // Calculate the steepness of the vertex
        float steepness = CalculateSteepness(normals[index]);

        // Penalize fitness for steep slopes
        if (steepness > maxSteepness)
        {
            fitness -= 0.7f;
        }
        // Penalize fitness if the vertex is below the water height
        if (vertices[index].y * meshScale < waterHeight + 1)
        {
            fitness -= 0.7f;
        }

        return fitness;
    }

    // Method to calculate the steepness of a vertex based on its normal
    private float CalculateSteepness(Vector3 normal)
    {
        // The up vector in the world space
        Vector3 up = Vector3.up;

        // Dot product between the normal and the up vector
        float dot = Vector3.Dot(normal.normalized, up);

        // Angle in degrees between the normal and the up vector
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle;
    }
}

[Serializable]
public struct TreePlacementData
{
    public Texture2D noiseMap; // Noise map texture used for procedural generation
    public float noiseScale;  // Scale of the noise map
    public int octaves; // Number of octaves for the noise generation
    public float lacunarity;  // Lacunarity value for the noise generation
    public float intensity; // Intensity of tree placement
    public float randomness; // Randomness factor for tree placement
    public float maxSteepness; // Maximum steepness of terrain where trees can be placed
    
    // Loads TreePlacementData from EditorPrefs using the specified save name
    public static TreePlacementData Load(string saveName)
    {
        TreePlacementData data;
        string saveData = EditorPrefs.GetString(saveName);

        if (string.IsNullOrEmpty(saveData))
        {
            // Default values if no saved data is found
            data = new TreePlacementData
            {
                noiseMap = null,
                noiseScale = 20f,
                octaves = 1,
                lacunarity = 1,
                intensity = 0.5f,
                randomness = 0.5f,
                maxSteepness = 30f,
            };
        }
        else
        {
            // Load data from JSON
            data = JsonUtility.FromJson<TreePlacementData>(saveData);
        }

        return data;
    }

    // Saves TreePlacementData to EditorPrefs using the specified save name
    public static void Save(string saveName, TreePlacementData data)
    {
        EditorPrefs.SetString(saveName, JsonUtility.ToJson(data));
    }
}