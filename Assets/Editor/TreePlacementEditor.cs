using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class TreePlacementEditor : EditorWindow
{
   private TreePlacementData data;
   public TreePlacement treePlacement;
   private TerrainGeneration terrainGeneration;
   public GameManager gameManager;

   private int newSeed;
   
   private static string treesSaveName
   {
      get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TreeData"; }
   }
   
   private static string terrainSaveName
   {
      get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TerrainData"; }
   }
   
   [MenuItem("Tools/Tree Placement")]
   public static void ShowWindow()
   {
      GetWindow<TreePlacementEditor>("Tree Placement");
   }

   private void OnEnable()
   {
      data = TreePlacementData.Load(treesSaveName);
   }

   private void OnDisable()
   {
      TreePlacementData.Save(treesSaveName, data);
   }

   public void OnGUI()
   {
      treePlacement =
         (TreePlacement)EditorGUILayout.ObjectField("Tree Placement", treePlacement, typeof(TreePlacement), true);
      terrainGeneration =  (TerrainGeneration)EditorGUILayout.ObjectField("Terrain Generation", terrainGeneration, typeof(TerrainGeneration), true);
      
      if (GUILayout.Button("Find References"))
      {
         treePlacement = FindObjectOfType<TreePlacement>();
         terrainGeneration = FindObjectOfType<TerrainGeneration>();
         gameManager = FindObjectOfType<GameManager>();
      }
      
      data.noiseMap =
         (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", data.noiseMap, typeof(Texture2D), false);
   
      data.noiseScale = EditorGUILayout.Slider("Noise Scale", data.noiseScale, 0, 100); 
      data.lacunarity  =  EditorGUILayout.Slider("Lacunarity", data.lacunarity, 0, 10);
      data.octaves =  EditorGUILayout.IntSlider("octaves", data.octaves, 0, 10);
      
      if (GUILayout.Button("Generate Noise"))
      {
         TerrainGenerationData terrainGenerationData = TerrainGenerationData.Load(terrainSaveName);
         int width = (int)terrainGenerationData.xSize;
         int height = (int)terrainGenerationData.zSize;
         data.noiseMap = Noise.GetNoiseMap(width, height, data.noiseScale, data.octaves, data.lacunarity, terrainGenerationData.seed,terrainGenerationData.randomiseSeed, out newSeed);
      }

      data.intensity = EditorGUILayout.Slider("Intensity", data.intensity, 0, 1);
      data.randomness = EditorGUILayout.Slider("Randomness", data.randomness, 0, 1);
      data.maxSteepness = EditorGUILayout.Slider("Max Steepness", data.maxSteepness, 0, 90);
      
      if (GUILayout.Button("Place Trees"))
      {
         TerrainGenerationData terrainGenerationData = TerrainGenerationData.Load(terrainSaveName);
         
         treePlacement.ClearTrees(data, gameManager);
         treePlacement.PlaceTrees(data, terrainGeneration, terrainGenerationData, gameManager);
      }

      if (GUILayout.Button("Clear Trees"))
      {
         treePlacement.ClearTrees(data, gameManager);
      }
   }
}

