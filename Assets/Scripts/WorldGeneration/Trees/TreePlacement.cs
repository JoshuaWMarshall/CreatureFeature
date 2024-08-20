using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TreePlacement : MonoBehaviour
{
    private TargetManager targetManager;
    private Dictionary<GameObject, bool> herbivoreFood = new Dictionary<GameObject, bool>();
    private GameObject restAreaContainer;
    private GameObject treeContainer;
    private GameManager _gameManager;
    private bool herbivoreFoodInitialised = false;
    private void Start()
    {
        targetManager = FindObjectOfType<TargetManager>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (_gameManager.gameStarted && !herbivoreFoodInitialised)
        {
            targetManager.InitializeHerbivoreFoodDict(herbivoreFood);
            herbivoreFoodInitialised = true;
        }
    }

    public void PlaceTrees(TreePlacementData data, TerrainGeneration terrainGeneration, TerrainGenerationData terrainGenerationData, GameManager manager)
    {
        if (_gameManager == null)
        {
            _gameManager = manager;
        }
        
        if (data.treePrefab == null)
        {
            data.treePrefab = Resources.Load("Poplar_Tree") as GameObject;
        }
        if (data.restAreaPrefab == null)
        {
            data.restAreaPrefab = Resources.Load("RestArea") as GameObject;
        }
        
        
        Vector3[] vertices = terrainGeneration.mesh.vertices;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Fitness(data.noiseMap, terrainGeneration.mesh, terrainGenerationData.meshScale, data.maxSteepness, terrainGenerationData.waterHeight, i) > 1 - data.intensity)
            {
                // Applying a random offset to position
                Vector3 pos = new Vector3(vertices[i].x + Random.Range(-data.randomness, data.randomness),
                    vertices[i].y,  vertices[i].z + Random.Range(-data.randomness, data.randomness)) * terrainGenerationData.meshScale;
                
                if (treeContainer == null)
                {
                    treeContainer = new GameObject();
                    treeContainer.name = "Tree Container";
                    treeContainer.tag = "TreeContainer";
                }
                
                GameObject clone = Instantiate(data.treePrefab, pos, Quaternion.identity, treeContainer.transform);
                clone.name = $"Poplar Tree #{i}";
                herbivoreFood.Add(clone, true);

                if (Random.Range(0, 5) < 1)
                {
                    if (restAreaContainer == null)
                    {
                        restAreaContainer = new GameObject();
                        restAreaContainer.name = "Rest Area Container";
                        restAreaContainer.tag = "RestAreaContainer";
                    }
                    
                    GameObject restArea = Instantiate(data.restAreaPrefab, pos + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)),
                        Quaternion.identity, restAreaContainer.transform);
                }
            }
        }
        
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }

        _gameManager.treesPlaced = true;
    }
    public void ClearTrees(TreePlacementData data, GameManager manager)
    {
        if (_gameManager == null)
        {
            _gameManager = manager;
        }
        
        if (restAreaContainer == null)
        {
            restAreaContainer = GameObject.FindGameObjectWithTag("RestAreaContainer");
        }

        if (treeContainer == null)
        {
            treeContainer = GameObject.FindGameObjectWithTag("TreeContainer");
        }
        
        DestroyImmediate(treeContainer);
        
        DestroyImmediate(restAreaContainer);
        
        if (herbivoreFood is { Count: > 0 })
        {
            herbivoreFood.Clear();
        }
        
        _gameManager.treesPlaced = false;
    }

    private float Fitness(Texture2D noiseMap, Mesh mesh, float meshScale, float maxSteepness, float waterHeight, int index)
    {
                
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
                
        float fitness = noiseMap.GetPixel((int)vertices[index].x, (int)vertices[index].z).g;
        
        float steepness = CalculateSteepness(normals[index]);

        // check for steep slopes
        if (steepness > maxSteepness)
        {
            fitness -= 0.7f;
        }
        // check if we are above water height
        if (vertices[index].y * meshScale < waterHeight + 1)
        {
            fitness -= 0.7f;
        }


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
public struct TreePlacementData
{
    // noise variable
    public Texture2D noiseMap;
    public float noiseScale;
    public int octaves;
    public float lacunarity;
    public GameObject treePrefab;
    public GameObject restAreaPrefab;
    
    // tree variable
    public float intensity;
    [FormerlySerializedAs("uniformity")] public float randomness;
    public float maxSteepness;
    
    public static TreePlacementData Load(string saveName)
    {
        TreePlacementData data;
        string saveData = EditorPrefs.GetString(saveName);

        if (string.IsNullOrEmpty(saveData))
        {
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
            data = JsonUtility.FromJson<TreePlacementData>(saveData);
        }

        return data;
    }

    public static void Save(string saveName, TreePlacementData data)
    {
        EditorPrefs.SetString(saveName, JsonUtility.ToJson(data));
    }
}
