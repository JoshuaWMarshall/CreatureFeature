using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    private GameObject waterMesh;
    public int MESH_SCALE = 100;
    public GameObject[] objects;
    [SerializeField] private AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;
    
    private Color[] colors;
    [SerializeField] private Gradient gradient;
    
    private float minTerrainheight;
    private float maxTerrainheight;

    public int xSize;
    public int zSize;

    public float scale; 
    public int octaves;
    public float lacunarity;

    public bool randomiseSeed = false;
    public int seed;

    private float lastNoiseHeight;
    public GameObject waterMeshPrefab;
    public float waterHeight = 100f;

    public GameObject stegosaurusContainer;
    public GameObject velociraptorContainer;
    public GameObject embelishmentsContainer;

    public GameObject[] dinosPefabs;
    public int maxDinos;
    public List<GAgent> allDinos = new List<GAgent>();
    private List<GameObject> allForrests = new List<GameObject>();

    void Start()
    {
        // Use this method if you havn't filled out the properties in the inspector
        // SetNullProperties(); 

        ClearLists();

        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }


        //LOAD
        //SaveLoad.LoadGame(this);

        //SaveLoad.SaveGame(this, allDinos);
    }

    private void ClearLists()
    {
        allDinos.Clear();
        allForrests.Clear();
    }

    private void SetNullProperties() 
    {
        if (xSize <= 0) xSize = 50;
        if (zSize <= 0) zSize = 50;
        if (octaves <= 0) octaves = 5;
        if (lacunarity <= 0) lacunarity = 2;
        if (scale <= 0) scale = 50;
    } 

    public void GenerateWorld()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateNewMap();
        MapEmbellishments();
    }

    public void DestroyWorld()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = null;   
        mesh = null;
        DestroyImmediate(waterMesh);

        while (stegosaurusContainer.transform.childCount != 0)
        {
            DestroyImmediate(stegosaurusContainer.transform.GetChild(0).gameObject);
        }

        while (velociraptorContainer.transform.childCount != 0)
        {
            DestroyImmediate(velociraptorContainer.transform.GetChild(0).gameObject);
        }


        while (embelishmentsContainer.transform.childCount != 0)
        {
            DestroyImmediate(embelishmentsContainer.transform.GetChild(0).gameObject);
        }
    }

    public void CreateNewMap()
    {
        CreateMeshShape();
        CreateTriangles();
        ColorMap();
        CreateWaterMesh(); // Create the water mesh
        UpdateMesh();

    }

    private void CreateMeshShape ()
    {
        // Creates seed
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0) scale = 0.0001f;
            
        // Create vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Set height of vertices
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                SetMinMaxHeights(noiseHeight);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }
    }

    private Vector2[] GetOffsetSeed()
    {
        if (randomiseSeed)
        {
            seed = Random.Range(0, 1000);
        }
 
        // changes area of map
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
                    
        for (int o = 0; o < octaves; o++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        float amplitude = 20;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        // loop over octaves
        for (int y = 0; y < octaves; y++)
        {
            float mapZ = z / scale * frequency + octaveOffsets[y].y;
            float mapX = x / scale * frequency + octaveOffsets[y].x;

            //The *2-1 is to create a flat floor level
            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight;
    }

    private void SetMinMaxHeights(float noiseHeight)
    {
        // Set min and max height of map for color gradient
        if (noiseHeight > maxTerrainheight)
            maxTerrainheight = noiseHeight;
        if (noiseHeight < minTerrainheight)
            minTerrainheight = noiseHeight;
    }


    private void CreateTriangles() 
    {
        // Need 6 vertices to create a square (2 triangles)
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        // Go to next row
        for (int z = 0; z < xSize; z++)
        {
            // fill row
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void ColorMap()
    {
        colors = new Color[vertices.Length];

        // Loop over vertices and apply a color from the depending on height (y axis value)
        for (int i = 0, z = 0; z < vertices.Length; z++)
        {
            float height = Mathf.InverseLerp(minTerrainheight, maxTerrainheight, vertices[i].y);
            colors[i] = gradient.Evaluate(height);
            i++;
        }
    }

    public void PlaceTrees()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if (System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > waterHeight + 0.1f)
                {
                    // Chance to generate trees
                    if (Random.Range(1, 5) == 1)
                    {
                        GameObject objectToSpawn = objects[Random.Range(0, objects.Length)];
                        var spawnAboveTerrainBy = noiseHeight * 2;
                        GameObject clone = Instantiate(objectToSpawn, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.identity, embelishmentsContainer.transform);

                        clone.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                        allForrests.Add(clone);
                    }
                }
            }
            lastNoiseHeight = noiseHeight;
        }
    }

    public GAgent PlaceDino(bool isStego)
    {
        GameObject clone;

        if (isStego)
        {
           clone = Instantiate(dinosPefabs[0], dinosPefabs[0].transform.position, Quaternion.identity, stegosaurusContainer.transform);
        }
        else if(dinosPefabs[1] !=  null) 
        {
           clone = Instantiate(dinosPefabs[1], dinosPefabs[1].transform.position, Quaternion.identity, velociraptorContainer.transform);
        }
        else
        {
            return null;
        }

        allDinos.Add(clone.GetComponent<GAgent>());

        return clone.GetComponent<GAgent>();
    }


    public void MapEmbellishments()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            // find actual position of vertices in the game
            Vector3 worldPt = transform.TransformPoint(mesh.vertices[i]);
            var noiseHeight = worldPt.y;
            // Stop generation if height difference between 2 vertices is too steep
            if (System.Math.Abs(lastNoiseHeight - worldPt.y) < 25)
            {
                // min height for object generation
                if (noiseHeight > waterHeight + 0.1f)
                {
                    // Chance to generate trees
                    if (Random.Range(1, 5) == 1)
                    {
                        GameObject objectToSpawn = objects[Random.Range(0, objects.Length)];
                        var spawnAboveTerrainBy = noiseHeight * 2;
                        GameObject clone = Instantiate(objectToSpawn, new Vector3(mesh.vertices[i].x * MESH_SCALE, spawnAboveTerrainBy, mesh.vertices[i].z * MESH_SCALE), Quaternion.identity, embelishmentsContainer.transform);

                        clone.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                        allForrests.Add(clone);
                    }
                }
            }
            lastNoiseHeight = noiseHeight;
        }

        for (int i = 0; i <= maxDinos; i++)
        {
            Vector3 worldPt = transform.TransformPoint(GetRandomInnerVertex());
            int randomInt = Random.Range(0, dinosPefabs.Length);
            GameObject objectToSpawn = dinosPefabs[randomInt];
            objectToSpawn.transform.position = worldPt + new Vector3(0, 100, 0);

            if (randomInt == 0)
            {
                GameObject clone = Instantiate(objectToSpawn, objectToSpawn.transform.position, Quaternion.identity, stegosaurusContainer.transform);
                clone.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                clone.name = $"Stegosaurus #{i}";
                allDinos.Add(clone.GetComponent<GAgent>());
            }
            else if (randomInt == 1)
            {
                GameObject clone = Instantiate(objectToSpawn, objectToSpawn.transform.position, Quaternion.identity, velociraptorContainer.transform);
                clone.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                clone.transform.localScale = Vector3.one * Random.Range(.8f, 1.2f);
                clone.name = $"Velociraptor #{i}";
                allDinos.Add(clone.GetComponent<GAgent>());
            }
        }
    }

    private Vector3 GetRandomInnerVertex()
    {
        // Ensure we are only working with inner vertices by avoiding the edges
        int xMin = 1;
        int xMax = xSize - 1;
        int zMin = 1;
        int zMax = zSize - 1;

        int x = Random.Range(xMin, xMax);
        int z = Random.Range(zMin, zMax);

        int vertexIndex = z * (xSize + 1) + x;
        return vertices[vertexIndex];
    }
    
    private void CreateWaterMesh()
    {
        if (waterMeshPrefab != null)
        {
            // *2 as water mesh is 50m, map mesh is 100m
            waterMesh = Instantiate(waterMeshPrefab, new Vector3((xSize * MESH_SCALE/2), waterHeight, (zSize * MESH_SCALE/2)), Quaternion.identity);
            waterMesh.transform.localScale = new Vector3(xSize * 2, 1, zSize * 2);
            waterMesh.layer = 4;

            NavMeshModifier navMeshModifier =  waterMesh.AddComponent<NavMeshModifier>();
            navMeshModifier.overrideArea = true;
            navMeshModifier.area = 1;
        }
        else
        {
            Debug.LogWarning("Water Mesh Prefab is not assigned.");
        }
    }

    public Vector3[] GetVertices()
    {
        return vertices;
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();


        GetComponent<MeshCollider>().sharedMesh = mesh;
        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE);
        gameObject.layer = 13;
    }

}
