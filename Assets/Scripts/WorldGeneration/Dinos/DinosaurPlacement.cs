using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class DinosaurPlacement : MonoBehaviour
{
    private Dictionary<GameObject, bool> carnivoreFood = new Dictionary<GameObject, bool>(); 
    private List<GAgent> allDinos = new List<GAgent>();
    private int vertexIndex;

    private GameObject stegoContainer;
    private GameObject raptorContainer;
    
    
    private TargetManager targetManager;
    private void Start()
    {
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        targetManager.InitializeCarnivoreFoodDict(carnivoreFood);
    }
    
    public void PlaceDinos(GameObject stegoPrefab, GameObject raptorPrefab, TerrainGeneration terrainGeneration, 
        TerrainGenerationData terrainGenerationData, DinosaurPlacementData data)
    {
        Vector3[] vertices = terrainGeneration.mesh.vertices;

        for (int i = 0; i < data.maxStegosaurus; i++)
        {
            Vector3 worldPt = Vector3.zero;
            bool validPosition = false;

            while (!validPosition)
            {
                Vector3 potentialPosition = GetRandomInnerVertex(vertices, terrainGenerationData.xSize, terrainGenerationData.zSize, out vertexIndex);
                
                if (Fitness(potentialPosition, terrainGeneration.mesh, terrainGenerationData.meshScale,
                        30, terrainGenerationData.waterHeight, typeof(Herbivore)) >= 1)
                {
                    worldPt = transform.TransformPoint(potentialPosition);
                    validPosition = true;
                }
            }

            if (stegoPrefab!= null)
            {
                if (stegoContainer == null)
                {
                    stegoContainer = new GameObject();
                    stegoContainer.name = "Stegosaurus Container";
                    stegoContainer.tag = "StegoContainer";
                }
                

                GameObject clone = Instantiate(stegoPrefab,  worldPt, Quaternion.identity, stegoContainer.transform);
                carnivoreFood.Add(clone, true);
            
                clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                clone.name = $"Stegosaurus #{i}";
                allDinos.Add(clone.GetComponent<GAgent>());
            }   
        }

        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        targetManager.InitializeCarnivoreFoodDict(carnivoreFood);
        
        for (int i = 0; i < data.maxVelociraptors; i++)
        {
            Vector3 worldPt = Vector3.zero;
            bool validPosition = false;

            while (!validPosition)
            {
                Vector3 potentialPosition = GetRandomInnerVertex(vertices, terrainGenerationData.xSize, terrainGenerationData.zSize, out vertexIndex);
                if (Fitness(potentialPosition, terrainGeneration.mesh, terrainGenerationData.meshScale,
                        30, terrainGenerationData.waterHeight, typeof(Carnivore)) >= 1)
                {
                    worldPt = transform.TransformPoint(potentialPosition);
                    validPosition = true;
                }
            }

            if (raptorPrefab != null)
            {
                if (raptorContainer == null)
                {
                    raptorContainer = new GameObject();
                    raptorContainer.name = "Velociraptor Container";
                    raptorContainer.tag = "RaptorContainer";
                }
                
                GameObject clone = Instantiate(raptorPrefab, worldPt, Quaternion.identity, raptorContainer.transform);
            
                clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                clone.name = $"Velociraptor #{i}";
                allDinos.Add(clone.GetComponent<GAgent>());
            }
        }
    }

    public void ClearDinos(DinosaurPlacementData data)
    {
        if (stegoContainer == null)
        {
            stegoContainer = GameObject.FindGameObjectWithTag("StegoContainer");
        }
        
        if (raptorContainer == null)
        {
            raptorContainer = GameObject.FindGameObjectWithTag("RaptorContainer");
        }
        
        DestroyImmediate(stegoContainer);
        
        DestroyImmediate(raptorContainer);

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
        
        // // Check for proximity to other dinosaurs of the same type
        // foreach (var dino in allDinos)
        // {
        //     if (dino.GetType() == dinoType)
        //     {
        //         float distance = Vector3.Distance(pos, dino.transform.position);
        //         if (distance < 100f) // Threshold distance for "close proximity"
        //         {
        //             fitness += 0.5f; // Increase fitness score for proximity
        //         }
        //     }
        // }

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
