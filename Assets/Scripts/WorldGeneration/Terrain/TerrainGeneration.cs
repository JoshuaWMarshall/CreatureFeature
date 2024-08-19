using System;
using UnityEngine;
using UnityEditor;

public class TerrainGeneration : MonoBehaviour
{
    // Flag to check if the terrain has been generated
    private bool isTerrainGenerated = false;
    
    // Mesh object to hold the generated terrain mesh
    [HideInInspector] public Mesh mesh;
    
    // Arrays to store vertices and triangles of the mesh
    private Vector3[] vertices;
    private int[] triangles;
    
    // Variables to store the minimum and maximum terrain heights
    private float minTerrainheight;
    private float maxTerrainheight;
    
    // Array to store the colors of the vertices
    private Color[] colours;

    // Variable to store the new seed value for noise generation
    private int newSeed;
    
    // Method to generate the terrain based on the provided data
    public void GenerateTerrain(TerrainGenerationData data)
    {
        // Initialize the mesh and assign it to the MeshFilter component
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        // Initialize the min and max terrain heights
        minTerrainheight = float.MaxValue;
        maxTerrainheight = float.MinValue;

        // Create the mesh shape, triangles, color map, and update the mesh
        CreateMeshShape(data);
        CreateTriangles(data);
        ColourMap(data);
        UpdateMesh(data);
        
        // Set the terrain generated flag to true
        isTerrainGenerated = true;
    }

    // Method to destroy the generated terrain
    public void DestroyTerrain(TerrainGenerationData data)
    {
        // Clear the mesh from the MeshFilter and MeshCollider components
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = null;
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        mesh = null;
        
        // Set the terrain generated flag to false
        isTerrainGenerated = false;
        
        // Destroy any existing water mesh
        if (data.waterMesh != null)
        {
            DestroyImmediate(data.waterMesh);
            data.waterMesh = null;
        }
        else
        {
            data.waterMesh = GameObject.FindGameObjectWithTag("Water");
            DestroyImmediate(data.waterMesh);
            data.waterMesh = null;
        }
    }
    
    // Method to create the shape of the mesh based on noise data
    private void CreateMeshShape(TerrainGenerationData data)
    {
        // Generate the noise map if it doesn't exist
        if (data.noiseMap == null)
        {
            data.noiseMap = Noise.GetNoiseMap(data.xSize, data.zSize, data.noiseScale, data.octaves, data.lacunarity, data.seed, data.randomiseSeed, out newSeed);
            data.seed = newSeed;
        }
        
        // Initialize the vertices array
        vertices = new Vector3[(data.xSize + 1) * (data.zSize + 1)];

        // Loop through each vertex and set its position based on the noise map
        for (int i = 0, z = 0; z <= data.zSize; z++)
        {
            for (int x = 0; x <= data.xSize; x++)
            {
                // Use the noise map to set the height of vertices
                float noiseHeight = data.noiseMap.GetPixel(x, z).g * data.heightCurve.Evaluate(1.0f) * data.heightMultiplier;
                SetMinMaxHeights(noiseHeight);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }
    }

    // Method to set the minimum and maximum heights of the terrain
    private void SetMinMaxHeights(float noiseHeight)
    {
        // Update the min and max height values
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }
    
    // Method to create the triangles of the mesh
    private void CreateTriangles(TerrainGenerationData data) 
    {
        // Initialize the triangles array
        triangles = new int[data.xSize * data.zSize * 6];

        int vert = 0;
        int tris = 0;
        
        // Loop through each square and create two triangles for each
        for (int z = 0; z < data.xSize; z++)
        {
            for (int x = 0; x < data.xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + data.xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + data.xSize + 1;
                triangles[tris + 5] = vert + data.xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }
    
    // Method to color the vertices of the mesh based on their height
    private void ColourMap(TerrainGenerationData data)
    {
        // Initialize the colors array
        colours = new Color[vertices.Length];

        // Loop through each vertex and set its color based on its height
        for (int i = 0, z = 0; z < vertices.Length; z++)
        {
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            colours[i] = data.gradient.Evaluate(height);
            i++;
        }
    }
    
    // Method to update the mesh with the generated vertices, triangles, and colors
    private void UpdateMesh(TerrainGenerationData data)
    {
        // Clear the existing mesh data
        mesh.Clear();
        
        // Assign the new vertices, triangles, and colors to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;
        
        // Recalculate the tangents and normals of the mesh
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        
        // Assign the mesh to the MeshCollider component
        GetComponent<MeshCollider>().sharedMesh = mesh;
        
        // Set the scale and layer of the game object
        gameObject.transform.localScale = new Vector3(data.meshScale, data.meshScale, data.meshScale);
        gameObject.layer = 13; // Terrain Layer = 13
    }
    
    // Method to create a water mesh based on the provided prefab
    public void CreateWaterMesh(TerrainGenerationData data, GameObject waterMeshPrefab)
    {
        // Destroy any existing water mesh
        if (data.waterMesh != null)
        {
            DestroyImmediate(data.waterMesh);
            data.waterMesh = null;
        }
        else
        {
            data.waterMesh = GameObject.FindGameObjectWithTag("Water");
            DestroyImmediate(data.waterMesh);
            data.waterMesh = null;
        }
        
        // Instantiate the new water mesh if the prefab is provided
        if (waterMeshPrefab != null)
        {
            data.waterMesh = Instantiate(waterMeshPrefab, new Vector3((data.xSize * data.meshScale/2), data.waterHeight, (data.zSize * data.meshScale/2)), Quaternion.identity);
            // *2 size as water mesh is 50m, map mesh is 100m
            data.waterMesh.transform.localScale = new Vector3(data.xSize * 2, 1, data.zSize * 2);
            data.waterMesh.layer = 4;
            data.waterMesh.tag = "Water";
        }
        else
        {
            Debug.LogWarning("Water Mesh Prefab is not assigned.");
        }
    }
}


[Serializable]
public struct TerrainGenerationData
{
    public bool randomiseSeed;
    public int seed;

    // mesh variables
    public int xSize;
    public int zSize;
    public int meshScale;
    public float heightMultiplier;

    public float waterHeight;
    public GameObject waterMesh;

    // mesh colour variables
    public AnimationCurve heightCurve;
    public Gradient gradient;

    // noise variables
    public Texture2D noiseMap;
    public float noiseScale;
    public int octaves;
    public float lacunarity;


    public static TerrainGenerationData Load(string saveName)
    {
        TerrainGenerationData data;

        // Retrieve the saved data from EditorPrefs using the saveName key
        string saveData = EditorPrefs.GetString(saveName);

        // Check if the retrieved data is null or empty
        if (string.IsNullOrEmpty(saveData))
        {
            // If no saved data is found, initialize a new TerrainGenerationData with default values
            data = new TerrainGenerationData
            {
                randomiseSeed = true,
                seed = 0,
                xSize = 50,
                zSize = 50,
                meshScale = 100,
                heightMultiplier = 5,
                waterHeight = 100,
                waterMesh = null,
                heightCurve = AnimationCurve.Linear(0, 0, 1, 1),
                gradient = CreateGradient(),
                noiseMap = null,
                noiseScale = 25f,
                octaves = 4,
                lacunarity = 2
            };
        }
        else
        {
            // If saved data is found, deserialize it into a TerrainGenerationData object
            data = JsonUtility.FromJson<TerrainGenerationData>(saveData);
        }

        // Return the loaded or newly created TerrainGenerationData
        return data;
    }

    public static void Save(string saveName, TerrainGenerationData data)
    {
        // Serialize the TerrainGenerationData object to JSON and save it in EditorPrefs using the saveName key
        EditorPrefs.SetString(saveName, JsonUtility.ToJson(data));
    }

    public static Gradient CreateGradient()
    {
        Gradient gradient = new Gradient();

        // Initialize color keys and alpha keys arrays for the gradient
        GradientColorKey[] colorKey = new GradientColorKey[3];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];

        // Set colors for the gradient
        colorKey[0].color = new Color(109, 70, 33);
        colorKey[0].time = 0.159f;
        colorKey[1].color = new Color(157, 123, 27);
        colorKey[1].time = 0.389f;
        colorKey[2].color = new Color(30, 99, 15);
        colorKey[2].time = 0.703f;

        // Set alpha values for the gradient
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.159f;
        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 0.389f;
        alphaKey[2].alpha = 1.0f;
        alphaKey[2].time = 0.703f;

        // Apply the color and alpha keys to the gradient
        gradient.SetKeys(colorKey, alphaKey);

        // Return the created gradient
        return gradient;
    }
}