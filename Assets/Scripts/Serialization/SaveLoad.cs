using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveLoad : MonoBehaviour
{
    private static string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    public static void SaveGame(MeshGenerator meshGen, List<GAgent> agents)
    {
        SaveData saveData = new SaveData
        {
            seed = meshGen.seed,
            waterHeight = meshGen.waterHeight,
            xSize = meshGen.xSize,
            zSize = meshGen.zSize,
            scale = meshGen.scale,
            octaves = meshGen.octaves,
            lacunarity = meshGen.lacunarity,
            meshScale = meshGen.MESH_SCALE,
            maxDinos = meshGen.maxDinos,
            dinosData = agents.Select(agent => new SaveData.DinoData
            {
                isStego = agent.CompareTag("Herbivore"),
                name = agent.name,
                hunger = agent.hunger,
                thirst = agent.thirst,
                energy = agent.energy,
                hungerRate = agent.hungerRate,
                thirstRate = agent.thirstRate,
                energyRate = agent.energyRate,
                position = agent.transform.position,
                rotation = agent.transform.rotation,
                state = agent.GetComponent<AgentStateManager>().currentAgentGoal.ToString()
            }).ToArray(),
            transformData = agents.Select(agent => new SaveData.TransformData
            {
                position = agent.transform.position,
                rotation = agent.transform.rotation
            }).ToArray()
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Game saved to " + savePath);
    }

    public static void LoadGame(MeshGenerator meshGen)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogError("Save file not found");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        // Load world data
        meshGen.seed = saveData.seed;
        meshGen.waterHeight = saveData.waterHeight;
        meshGen.xSize = saveData.xSize;
        meshGen.zSize = saveData.zSize;
        meshGen.scale = saveData.scale;
        meshGen.octaves = saveData.octaves;
        meshGen.lacunarity = saveData.lacunarity;
        meshGen.MESH_SCALE = saveData.meshScale;
        meshGen.CreateNewMap();
        // Place Trees 
        meshGen.PlaceTrees();
     
        // Load dinosaur data
        for (int i = 0; i < saveData.dinosData.Length; i++)
        {
            var data = saveData.dinosData[i];

            GAgent agent = meshGen.PlaceDino(data.isStego);

            agent.name = data.name;
            agent.hunger = data.hunger;
            agent.thirst = data.thirst;
            agent.energy = data.energy;
            agent.hungerRate = data.hungerRate;
            agent.thirstRate = data.thirstRate;
            agent.energyRate = data.energyRate;
            agent.transform.position = data.position;
            agent.transform.rotation = data.rotation;
            var stateManager = agent.GetComponent<AgentStateManager>();
            if (stateManager != null)
            {
                if (Enum.TryParse(data.state, out AgentGoal goal))
                {
                    stateManager.SwitchState(goal);
                }
            }
        }
    }

}
