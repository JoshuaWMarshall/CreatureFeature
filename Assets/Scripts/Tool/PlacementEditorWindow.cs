using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlacementEditorWindow : EditorWindow
{
   private Texture2D noiseMapTexture;
   private GameObject prefab;
   private PlacementGenes genes;

   private static string GenesSaveName
   {
      get { return $"_{Application.productName}_{EditorSceneManager.GetActiveScene().name}"; }
   }

   [MenuItem("Tools/ Tree Placement")]
   public static void ShowWindow()
   {
      GetWindow<PlacementEditorWindow>("Tree Placement");
   }

   private void OnEnable()
   {
      genes = PlacementGenes.Load(GenesSaveName);
   }

   private void OnDisable()
   {
      PlacementGenes.Save(GenesSaveName, genes);
   }

   private void OnGUI()
   {
      EditorGUILayout.BeginHorizontal();
      noiseMapTexture =
         (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", noiseMapTexture, typeof(Texture2D), false);
      if (GUILayout.Button("Generate Noise"))
      {
         int width = (int)Terrain.activeTerrain.terrainData.size.x;
         int height = (int)Terrain.activeTerrain.terrainData.size.y;
         float scale = 5;
         noiseMapTexture = Noise.GetNoiseMap(width, height, scale);
      }

      EditorGUILayout.EndHorizontal();

      genes.maxHeight = EditorGUILayout.Slider("Max Height", genes.maxHeight, 0, 1000);
      genes.maxSteepness = EditorGUILayout.Slider("Max Steepness", genes.maxSteepness, 0, 90);

      genes.density = EditorGUILayout.Slider("Density", genes.density, 0, 1);

      prefab = (GameObject)EditorGUILayout.ObjectField("Object Prefab", prefab, typeof(GameObject), false);

      if (GUILayout.Button("Place Object"))
      {
         PlaceObjects(Terrain.activeTerrain, noiseMapTexture, genes, prefab);
      }
   }

   public static void PlaceObjects(Terrain terrain, Texture2D noiseMapTexture, PlacementGenes genes, GameObject prefab)
   {
      Transform parent = new GameObject("PlacedObjects").transform;

      for (int x = 0; x < terrain.terrainData.size.x; x++)
      {
         for (int z = 0; z < terrain.terrainData.size.x; z++)
         {
            float noiseMapValue = noiseMapTexture.GetPixel(x, z).g;

            // if the value is above the threshold, instantiate a tree prefab at this location
            if (Fitness(terrain, noiseMapTexture, genes, x, z) > 1 - genes.density)
            {
               Vector3 pos = new Vector3(x + Random.Range(-0.5f, 0.5f), 0, z + Random.Range(-0.5f, 0.5f));
               pos.y = terrain.SampleHeight(new Vector3(x, 0, z));

               GameObject go = Instantiate(prefab, pos, Quaternion.identity);
               go.transform.SetParent(parent);
            }
         }
      }
   }

   private static float Fitness(Terrain terrain, Texture2D noiseMapTexture, PlacementGenes genes, int x, int z)
   {
      float fitness = noiseMapTexture.GetPixel(x, z).g;

      fitness += Random.Range(-0.25f, 0.25f);

      float steepness =
         terrain.terrainData.GetSteepness(x / terrain.terrainData.size.x, z / terrain.terrainData.size.z);
      if (steepness > 30)
      {
         fitness -= 0.7f;
      }


      return fitness;
   }

   [Serializable]
   public struct PlacementGenes
   {
      public float density;
      public float maxHeight;
      public float maxSteepenss;
      internal static PlacementGenes Load(string saveName)
      {
         PlacementGenes genes;
         string saveData = EditorPrefs.GetString(saveName);

         if (string.IsNullOrEmpty(saveData))
         {
            genes = new PlacementGenes();
            genes.density = 0.5f;
            genes.maxHeight = 100;
            genes.maxSteepenss = 25;
         }
         else
         {
            genes = JsonUtility.FromJson<PlacementGenes>(saveData);
         }

         return genes;
      }

      internal static void Save(string saveName, PlacementGenes genes)
      {
         EditorPrefs.SetString(saveName, JsonUtility.ToJson(genes));
      }
   }
}

/*
 * using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlacementEditorWindow : EditorWindow
{
private Texture2D noiseMapTexture;
private GameObject prefab;
private PlacementGenes genes;

[MenuItem("Tools/ Tree Placement")]
public static void ShowWindow()
{
   GetWindow<PlacementEditorWindow>("Tree Placement");
}

private void OnEnable()
{
   genes = new PlacementGenes();
   genes.density = 0.5f;
   genes.maxHeight = 100;
   genes.maxSteepness = 25;
}
private void OnGUI()
{
   EditorGUILayout.BeginHorizontal();
   noiseMapTexture = (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", noiseMapTexture, typeof(Texture2D), false);
   if (GUILayout.Button("Generate Noise"))
   {
      int width = (int)Terrain.activeTerrain.terrainData.size.x;
      int height = (int)Terrain.activeTerrain.terrainData.size.y;
      float scale = 5;
      noiseMapTexture = Noise.GetNoiseMap(width, height, scale);
   }
   EditorGUILayout.EndHorizontal();

   genes.maxHeight = EditorGUILayout.Slider("Max Height", genes.maxHeight, 0, 1000);
   genes.maxSteepness = EditorGUILayout.Slider("Max Steepness", genes.maxSteepness, 0, 90);

   genes.density = EditorGUILayout.Slider("Density", genes.density, 0, 1);

   prefab = (GameObject)EditorGUILayout.ObjectField("Object Prefab", prefab, typeof(GameObject), false);

   if (GUILayout.Button("Place Object"))
   {
      PlaceObjects(Terrain.activeTerrain, noiseMapTexture, genes, prefab);
   }
}

public static void PlaceObjects(Terrain terrain, Texture2D noiseMapTexture, PlacementGenes genes, GameObject prefab)
{
   Transform parent = new GameObject("PlacedObjects").transform;

   for (int x = 0; x < terrain.terrainData.size.x; x++)
   {
      for (int z = 0; z < terrain.terrainData.size.x; z++)
      {
         float noiseMapValue = noiseMapTexture.GetPixel(x, z).g;

         // if the value is above the threshold, instantiate a tree prefab at this location
         if (Fitness(terrain, noiseMapTexture, genes, x, z) > 1 - genes.density)
         {
            Vector3 pos = new Vector3(x + Random.Range(-0.5f, 0.5f), 0 , z + Random.Range(-0.5f, 0.5f));
            pos.y = terrain.SampleHeight(new Vector3(x, 0, z));

            GameObject go = Instantiate(prefab, pos, Quaternion.identity);
            go.transform.SetParent(parent);
         }
      }
   }
}

private static float Fitness(Terrain terrain, Texture2D noiseMapTexture, PlacementGenes genes,  int x, int z)
{
   float fitness = noiseMapTexture.GetPixel(x, z).g;

   fitness += Random.Range(-0.25f, 0.25f);

   float steepness =
      terrain.terrainData.GetSteepness(x / terrain.terrainData.size.x, z / terrain.terrainData.size.z);
   if (steepness > 30)
   {
      fitness -= 0.7f;
   }

   float height = terrain.terrainData.GetHeight(x, z);
   if(height > PlacementGenes.maxHeight)
   {
      fitness -= 0.7f;
   }

   return fitness;
}

[Serializable]
public struct PlacementGenes
{
   public float density;
   public float maxHeight;
   public float maxSteepenss;
}
}

 */
}
