using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
public class TerrainGenerationEditor : EditorWindow
{
    private TerrainGenerationData data;
    public TerrainGeneration terrainGeneration;
    private int newSeed;
    private GameObject waterMeshPrefab;
    
    private string[] terrainTypes = { "Custom","Big Hill", "Valley", "Rolling Hills" };
    private int selectedTerrainTypeIndex = 0;
    private bool isCustomTerrain;
    
    private static string terrainSaveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TerrainData"; }
    }
    
    [MenuItem("Tools/Terrain Generation")]
    public static void ShowWindow()
    {
        GetWindow<TerrainGenerationEditor>("Terrain Generation");
    }
    
    private void OnEnable()
    {
        data = TerrainGenerationData.Load(terrainSaveName);
    }

    private void OnDisable()
    {
        TerrainGenerationData.Save(terrainSaveName, data);
    }

    public void OnGUI()
    {
        terrainGeneration = (TerrainGeneration)EditorGUILayout.ObjectField("Terrain Generation", terrainGeneration,
            typeof(TerrainGeneration), true);
       
        if (GUILayout.Button("Find References"))
        {
            terrainGeneration = FindObjectOfType<TerrainGeneration>();
        }
        
        selectedTerrainTypeIndex = EditorGUILayout.Popup("Terrain Presets", selectedTerrainTypeIndex, terrainTypes);
        SetTerrainTypeValues(selectedTerrainTypeIndex);
        
        // Monitor changes in terrain properties
        EditorGUI.BeginChangeCheck();
        
        data.randomiseSeed = EditorGUILayout.Toggle("Randomise Seed", data.randomiseSeed);
        data.seed = EditorGUILayout.IntField("Seed", data.seed);
        
        data.noiseMap =
            (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", data.noiseMap, typeof(Texture2D), false);
        
        data.noiseScale = EditorGUILayout.Slider("Noise Scale", data.noiseScale, 0, 100); 
        data.lacunarity  =  EditorGUILayout.Slider("Lacunarity", data.lacunarity, 0, 10);
        data.octaves =  EditorGUILayout.IntSlider("octaves", data.octaves, 0, 10);
        
        if (GUILayout.Button("Generate Noise"))
        {
            int width = (int)(data.xSize);
            int height = (int)(data.zSize);
            data.noiseMap = Noise.GetNoiseMap(width, height, data.noiseScale, data.octaves, data.lacunarity, data.seed,
                data.randomiseSeed, out newSeed);
            data.seed = newSeed;
        }
        
        data.heightCurve = EditorGUILayout.CurveField("Height Curve", data.heightCurve);
        data.gradient = EditorGUILayout.GradientField("Gradient", data.gradient);
        
        data.xSize = EditorGUILayout.IntField("Mesh X Size", data.xSize);
        data.zSize =  EditorGUILayout.IntField("Mesh Z Size", data.zSize);
        data.meshScale =  EditorGUILayout.IntField("Mesh Scale", data.meshScale);
        data.heightMultiplier =  EditorGUILayout.Slider("Height Multiplier", data.heightMultiplier, 1, 10);

        waterMeshPrefab =
            (GameObject)EditorGUILayout.ObjectField("Water Mesh Prefab", waterMeshPrefab, typeof(GameObject), false);
        data.waterHeight = EditorGUILayout.FloatField("Water Height", data.waterHeight);
        
        // If any terrain property was changed, switch to "Custom"
        if (EditorGUI.EndChangeCheck())
        {
            selectedTerrainTypeIndex = 0; // Set to "Custom"
            isCustomTerrain = true;
        }
        
        if (GUILayout.Button("Generate Terrain"))
        {
            terrainGeneration.GenerateTerrain(data);
            terrainGeneration.CreateWaterMesh(data, waterMeshPrefab);
        }

        if (GUILayout.Button("Destroy Terrain"))
        {
            terrainGeneration.DestroyTerrain(data);
        }
    }
    
    private void SetTerrainTypeValues(int index)
    {
        if (!isCustomTerrain)
        {
            switch (index)
            {
                case 1: //Big Hill
                    data.noiseScale = 25f;
                    data.lacunarity = 2f;
                    data.octaves = 4;
                    data.heightMultiplier = 6f;
                    data.heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    break;
                case 2: //Valley
                    data.noiseScale = 15f;
                    data.lacunarity = 1.5f;
                    data.octaves = 3;
                    data.heightMultiplier = 4f;
                    data.heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    break;
                case 3: //Rolling Hills
                    data.noiseScale = 35f;
                    data.lacunarity = 1f;
                    data.octaves = 2;
                    data.heightMultiplier = 2f;
                    data.heightCurve = AnimationCurve.Linear(0, 0, 1, 1);
                    break;
            }
        }
        
        // Reset custom flag if preset is selected
        if (index != 0)
        {
            isCustomTerrain = false;
        }
    }
}
