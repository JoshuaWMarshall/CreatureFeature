using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class DinosaurPlacementEditor : EditorWindow
{
    private DinosaurPlacementData data;
    private TerrainGeneration terrainGeneration;
    private DinosaurPlacement dinosaurPlacement;
    private GameObject stegoPrefab;
    private GameObject stegoContainer;
    private GameObject raptorPrefab;
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
        }
        
        stegoPrefab = (GameObject)EditorGUILayout.ObjectField("Stegosaurus Prefab", stegoPrefab, typeof(GameObject), false);
        raptorPrefab = (GameObject)EditorGUILayout.ObjectField("Velociraptor Prefab", raptorPrefab, typeof(GameObject), false);
        data.maxStegosaurus = EditorGUILayout.IntField("Max Stegosaurus", data.maxStegosaurus);
        data.maxVelociraptors = EditorGUILayout.IntField("Max Velociraptors", data.maxVelociraptors);

        if (GUILayout.Button("Place Dinosaurs"))
        {
            TerrainGenerationData terrainGenerationData = TerrainGenerationData.Load(terrainSaveName);
            
            dinosaurPlacement.PlaceDinos(stegoPrefab, raptorPrefab, terrainGeneration, terrainGenerationData, data);
        }
        
        if (GUILayout.Button("Clear Dinosaurs"))
        {
            dinosaurPlacement.ClearDinos(data);
        }
    }
}
