using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class TreePlacement : MonoBehaviour
{
    private TargetManager targetManager;

    public void PlaceTrees(TreePlacementData data, TerrainGeneration terrainGeneration, TerrainGenerationData terrainGenerationData, Dictionary<GameObject, bool> herbivoreFood, GameObject treePrefab)
    {
        ClearTrees(data, herbivoreFood);

        Vector3[] vertices = terrainGeneration.mesh.vertices;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            if (Fitness(data.noiseMap, terrainGeneration.mesh, terrainGenerationData.meshScale, data.maxSteepness, terrainGenerationData.waterHeight, i) > 1 - data.intensity)
            {
                // Applying a random offset to position
                Vector3 pos = new Vector3(vertices[i].x + Random.Range(-data.randomness, data.randomness),
                    vertices[i].y,  vertices[i].z + Random.Range(-data.randomness, data.randomness)) * terrainGenerationData.meshScale;
                
                GameObject clone = Instantiate(treePrefab, pos, Quaternion.identity, data.parent);
                clone.name = $"Poplar Tree #{i}";
                herbivoreFood.Add(clone, true);
            }
        }
        
        if (targetManager == null)
        {
            targetManager = FindObjectOfType<TargetManager>();
        }
        
        targetManager.InitializeherbivoreFoodDict(herbivoreFood);
    }
    public void ClearTrees(TreePlacementData data, Dictionary<GameObject, bool> herbivoreFood)
    {
        // clear any active children to avoid duplicates
        while (data.parent.childCount != 0)
        {
            DestroyImmediate(data.parent.GetChild(0).gameObject);
        }

        if (herbivoreFood.Count > 0)
        {
            herbivoreFood.Clear();
        }
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
    
    // tree variable
    public float intensity;
    [FormerlySerializedAs("uniformity")] public float randomness;
    public float maxSteepness;
    public Transform parent;
    
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
         
                parent = GameObject.Find("Tree Container").transform
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
