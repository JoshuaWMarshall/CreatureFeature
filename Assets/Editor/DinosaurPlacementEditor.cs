using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class DinosaurPlacementEditor : EditorWindow
{
    public GameManager gameManager;
    private DinosaurPlacementData data;
    private TerrainGeneration terrainGeneration;
    private DinosaurPlacement dinosaurPlacement;
    private GameObject stegoContainer;
    private GameObject raptorContainer;
    
    private static string saveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_DinoData"; }
    }
    
    private static string terrainSaveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TerrainData"; }
    }
    
    [MenuItem("Tools/Dinosaur Placement")]
    public static void ShowWindow()
    {
        GetWindow<DinosaurPlacementEditor>("Dinosaur Placement");
    }
    
    private void OnEnable()
    {
        data = DinosaurPlacementData.Load(saveName);
    }

    private void OnDisable()
    {
        DinosaurPlacementData.Save(saveName, data);
    }

    public void OnGUI()
    {
        terrainGeneration = (TerrainGeneration)EditorGUILayout.ObjectField("Terrain Generation", terrainGeneration, typeof(TerrainGeneration), true);
        dinosaurPlacement = (DinosaurPlacement)EditorGUILayout.ObjectField("Dinosaur Placement", dinosaurPlacement, typeof(DinosaurPlacement), true); 
        
        if (GUILayout.Button("Find References"))
        {
            terrainGeneration = FindObjectOfType<TerrainGeneration>();
            dinosaurPlacement = FindObjectOfType<DinosaurPlacement>();
            gameManager = FindObjectOfType<GameManager>();
        }
        data.maxStegosaurus = EditorGUILayout.IntField("Max Stegosaurus", data.maxStegosaurus);
        data.maxVelociraptors = EditorGUILayout.IntField("Max Velociraptors", data.maxVelociraptors);

        if (GUILayout.Button("Place Dinosaurs"))
        {
            TerrainGenerationData terrainGenerationData = TerrainGenerationData.Load(terrainSaveName);
            
            dinosaurPlacement.PlaceDinos(terrainGeneration, terrainGenerationData, data, gameManager);
        }
        
        if (GUILayout.Button("Clear Dinosaurs"))
        {
            dinosaurPlacement.ClearDinos(data, gameManager);
        }
    }
}
