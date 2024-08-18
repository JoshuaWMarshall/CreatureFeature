using System;
using UnityEngine;
using UnityEditor;

public class TerrainGeneration : MonoBehaviour
{
    private bool isTerrainGenerated = false;
    [HideInInspector] public Mesh mesh;
    
    private Vector3[] vertices;
    private int[] triangles;
    
    private float minTerrainheight;
    private float maxTerrainheight;
    private Color[] colours;

    private int newSeed;
    
    public void GenerateTerrain(TerrainGenerationData data)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        minTerrainheight = float.MaxValue;
        maxTerrainheight = float.MinValue;

        CreateMeshShape(data);
        CreateTriangles(data);
        ColourMap(data);
        UpdateMesh(data);
        
        isTerrainGenerated = true;
    }

    public void DestroyTerrain(TerrainGenerationData data)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = null;
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = null;
        mesh = null;
        isTerrainGenerated = false;
        
        //Destroy dinos
        
        // Destroy water mesh
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

        //Destroy trees
    }
    
    private void CreateMeshShape(TerrainGenerationData data)
    {
        // Get the noise map texture
        if (data.noiseMap == null)
        {
            data.noiseMap = Noise.GetNoiseMap(data.xSize, data.zSize, data.noiseScale, data.octaves, data.lacunarity, data.seed, data.randomiseSeed, out newSeed);
            data.seed = newSeed;
        }
        
        vertices = new Vector3[(data.xSize + 1) * (data.zSize + 1)];

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
    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }
    
    private void CreateTriangles(TerrainGenerationData data) 
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[data.xSize * data.zSize * 6];

        int vert = 0;
        int tris = 0;
        // Go to next row
        for (int z = 0; z < data.xSize; z++)
        {
            // fill row
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
    
    private void ColourMap(TerrainGenerationData data)
    {
        colours = new Color[vertices.Length];

        // Loop over vertices and apply a color from the depending on height (y axis value)
        for (int i = 0, z = 0; z < vertices.Length; z++)
        {
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            colours[i] = data.gradient.Evaluate(height);
            i++;
        }
    }
    
    private void UpdateMesh(TerrainGenerationData data)
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        
        GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.transform.localScale = new Vector3(data.meshScale, data.meshScale, data.meshScale);
        // Terrain Layer = 13
        gameObject.layer = 13;
    }
    
    public void CreateWaterMesh(TerrainGenerationData data, GameObject waterMeshPrefab)
    {
        // Clear old mesh if it exists
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
        
        if (waterMeshPrefab != null)
        {
            // *2 size as water mesh is 50m, map mesh is 100m
            data.waterMesh = Instantiate(waterMeshPrefab, new Vector3((data.xSize * data.meshScale/2), data.waterHeight, (data.zSize * data.meshScale/2)), Quaternion.identity);
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
    //public Vector3[] vertices;
    public Vector3[] normals;
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
        string saveData = EditorPrefs.GetString(saveName);

        if (string.IsNullOrEmpty(saveData))
        {
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
            data = JsonUtility.FromJson<TerrainGenerationData>(saveData);
        }

        return data;
    }

    public static void Save(string saveName, TerrainGenerationData data)
    {
        EditorPrefs.SetString(saveName, JsonUtility.ToJson(data));
    }

    public static Gradient CreateGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKey = new GradientColorKey[3];
        GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];

        // Set colors
        colorKey[0].color = new Color(109, 70, 33);
        colorKey[0].time = 0.159f;
        colorKey[1].color = new Color(157, 123, 27);
        colorKey[1].time = 0.389f;
        colorKey[2].color = new Color(30, 99, 15);
        colorKey[2].time = 0.703f;

        // Set alphas
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.159f;
        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 0.389f;
        alphaKey[2].alpha = 1.0f;
        alphaKey[2].time = 0.703f;

        gradient.SetKeys(colorKey, alphaKey);

        return gradient;
    }

    // public Vector3[] GetVertices()
    // {
    //     return vertices;
    // }
    //
    // public Vector3[] GetNormals()
    // {
    //     return normals;
    // }
}