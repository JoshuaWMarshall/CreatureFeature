using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class DinosaurPlacement : MonoBehaviour
{
    // Dictionary to keep track of carnivore food sources
    private Dictionary<GameObject, bool> carnivoreFood = new Dictionary<GameObject, bool>(); 
    
    // List to store all dinosaur agents
    private List<GAgent> allDinos = new List<GAgent>();
    
    // Index of the vertex being processed
    private int vertexIndex;

    // Containers for different types of dinosaurs
    private GameObject stegoContainer;
    private GameObject raptorContainer;
    
    // Reference to the TargetManager
    private TargetManager targetManager;

    private void Start()
    {
        // Initialize the TargetManager if it is not already set
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        // Initialize the carnivore food dictionary in the TargetManager
        targetManager.InitializeCarnivoreFoodDict(carnivoreFood);
    }
    
    public void PlaceDinos(GameObject stegoPrefab, GameObject raptorPrefab, TerrainGeneration terrainGeneration, 
        TerrainGenerationData terrainGenerationData, DinosaurPlacementData data)
    {
        // Get the vertices of the terrain mesh
        Vector3[] vertices = terrainGeneration.mesh.vertices;

        // Place Stegosaurus dinosaurs
        for (int i = 0; i < data.maxStegosaurus; i++)
        {
            Vector3 worldPt = Vector3.zero;
            bool validPosition = false;

            // Find a valid position for the Stegosaurus
            while (!validPosition)
            {
                Vector3 potentialPosition = GetRandomInnerVertex(vertices, terrainGenerationData.xSize, terrainGenerationData.zSize, out vertexIndex);
                
                // Check if the position is suitable for a herbivore
                if (Fitness(potentialPosition, terrainGeneration.mesh, terrainGenerationData.meshScale,
                        30, terrainGenerationData.waterHeight, typeof(Herbivore)) >= 1)
                {
                    worldPt = transform.TransformPoint(potentialPosition);
                    validPosition = true;
                }
            }

            // Check if the container is initialised, if not create one
            if (stegoPrefab != null)
            {
                if (stegoContainer == null)
                {
                    stegoContainer = new GameObject();
                    stegoContainer.name = "Stegosaurus Container";
                    stegoContainer.tag = "StegoContainer";
                }
                // Instantiate the Stegosaurus prefab at the valid position
                GameObject clone = Instantiate(stegoPrefab, worldPt, Quaternion.identity, stegoContainer.transform);
                carnivoreFood.Add(clone, true);
            
                clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                clone.name = $"Stegosaurus #{i}";
                allDinos.Add(clone.GetComponent<GAgent>());
            }   
        }

        // Ensure the TargetManager is initialized
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        // Place Velociraptor dinosaurs
        for (int i = 0; i < data.maxVelociraptors; i++)
        {
            Vector3 worldPt = Vector3.zero;
            bool validPosition = false;

            // Find a valid position for the Velociraptor
            while (!validPosition)
            {
                Vector3 potentialPosition = GetRandomInnerVertex(vertices, terrainGenerationData.xSize, terrainGenerationData.zSize, out vertexIndex);
                
                // Check if the position is suitable for a carnivore
                if (Fitness(potentialPosition, terrainGeneration.mesh, terrainGenerationData.meshScale,
                        30, terrainGenerationData.waterHeight, typeof(Carnivore)) >= 1)
                {
                    worldPt = transform.TransformPoint(potentialPosition);
                    validPosition = true;
                }
            }

            // Check if the raptor container is initialised, if not create one
            if (raptorPrefab != null)
            {
                if (raptorContainer == null)
                {
                    raptorContainer = new GameObject();
                    raptorContainer.name = "Velociraptor Container";
                    raptorContainer.tag = "RaptorContainer";
                }
                // Instantiate the Velociraptor prefab at the valid position
                GameObject clone = Instantiate(raptorPrefab, worldPt, Quaternion.identity, raptorContainer.transform);
            
                clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                clone.name = $"Velociraptor #{i}";
                allDinos.Add(clone.GetComponent<GAgent>());
            }
        }
    }

    public void ClearDinos(DinosaurPlacementData data)
    {
        // Find and destroy the Stegosaurus container if it exists
        if (stegoContainer == null)
        {
            stegoContainer = GameObject.FindGameObjectWithTag("StegoContainer");
        }
        DestroyImmediate(stegoContainer);
        // Find and destroy the Velociraptor container if it exists
        if (raptorContainer == null)
        {
            raptorContainer = GameObject.FindGameObjectWithTag("RaptorContainer");
        }
        DestroyImmediate(raptorContainer);

        // Clear the carnivore food dictionary
        if (carnivoreFood.Count > 0)
        {
            carnivoreFood.Clear();
        }
    }
    
    private Vector3 GetRandomInnerVertex(Vector3[] vertices, int xSize, int zSize, out int vertexIndex)
    {
        // Ensure we are only working with inner vertices by avoiding the edges
        int xMin = 1;
        int xMax = xSize - 1;
        int zMin = 1;
        int zMax = zSize - 1;

        int x = Random.Range(xMin, xMax);
        int z = Random.Range(zMin, zMax);

        vertexIndex = z * (xSize + 1) + x;
        return vertices[vertexIndex];
    }

    private float Fitness(Vector3 pos, Mesh mesh, float meshScale, float maxSteepness, float waterHeight, Type dinoType)
    {
        // Scale the vertex to world coordinates
        float worldHeight = pos.y * meshScale;
        Vector3[] normals = mesh.normals;
        float steepness = CalculateSteepness(normals[vertexIndex]);
        
        float fitness = 1f;
        
        // Check if the vertex is above the water height
        if (worldHeight < waterHeight)
        {
            // If below water, return a fitness value of 0 (indicating a bad position)
            fitness -= 1f;
        }
        
        // Check if the vertex is on a slope
        if (steepness > maxSteepness)
        {
            fitness -= 0.3f;
        }
        
        // can add more checks here for more refined placement

        return fitness;
    }
    
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
public struct DinosaurPlacementData
{
    public int maxStegosaurus;
    public int maxVelociraptors;
    public static DinosaurPlacementData Load(string saveName)
    {
        DinosaurPlacementData data;
        string saveData = EditorPrefs.GetString(saveName);

        if (string.IsNullOrEmpty(saveData))
        {
            data = new DinosaurPlacementData
            {
                maxStegosaurus = 10,
                maxVelociraptors = 2
            };
        }
        else
        {
            data = JsonUtility.FromJson<DinosaurPlacementData>(saveData);
        }

        return data;
    }

    public static void Save(string saveName, DinosaurPlacementData data)
    {
        EditorPrefs.SetString(saveName, JsonUtility.ToJson(data));
    }
}
