using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    private MeshGenerator meshGenerator;

    private void OnEnable()
    {
        meshGenerator = (MeshGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }

        if(GUILayout.Button("Destroy World"))
        {
            DestroyWorld();
        }

        if (GUILayout.Button("Save"))
        {
            SaveGame();
        }

        if (GUILayout.Button("Load"))
        {
            LoadGame();
        }
    }

    private void Generate()
    {
        meshGenerator.GenerateWorld();
        Debug.Log("World Generated");
    }

    private void DestroyWorld()
    {
        meshGenerator.DestroyWorld();
        Debug.Log("World Destroyed");
    }

    private void SaveGame()
    {

        // Call SaveLoad.SaveGame to save game data
        SaveLoad.SaveGame(meshGenerator, meshGenerator.allDinos);
        Debug.Log("Game data saved.");
    }

    private void LoadGame()
    {
        // Call SaveLoad.LoadGame to load game data
        SaveLoad.LoadGame(meshGenerator);
        Debug.Log("Game data loaded.");
    }
}
