using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.SocialPlatforms;

public class GameManager : MonoBehaviour
{
    private DinoViewerPresenter dinoViewerPresenter;
    private VisualElement dinoViewerVisualElement;

    private WorldGeneratorPresenter worldGeneratorPresenter;
    private VisualElement worldGeneratorVisualElement;
    
    private GameObject stegosaurusContainer;
    private GameObject velociraptorContainer;
    private List<GameObject> stegosaurus;
    private List<GameObject> velociraptor;

    [HideInInspector] public GAgent currentDino;
    [HideInInspector] public CameraOrbit cameraOrbit;
    
    public TerrainGenerationData terrainGenerationData;
    [HideInInspector] public TerrainGeneration terrainGeneration;
    public TreePlacementData treePlacementData;
    [HideInInspector] public TreePlacement treePlacement;
    public DinosaurPlacementData dinoPlacementData;
    [HideInInspector] public DinosaurPlacement dinoPlacement;
    private TargetManager targetManager;
    
    public bool meshGenerated = false;
    public bool treesPlaced = false;
    public bool dinosPlaced = false;
    public bool gameStarted = false;
    
    private static string terrainSaveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TerrainData"; }
    }
    
    private static string treesSaveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_TreeData"; }
    }
    
    private static string dinoSaveName
    {
        get { return $"{Application.productName}_{EditorSceneManager.GetActiveScene().name}_DinoData"; }
    }

    
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        dinoViewerVisualElement = root.Q("DinoViewer");
        worldGeneratorVisualElement = root.Q("WorldGenerator");
    }
    
    private void Awake()
    {
        terrainGeneration = FindObjectOfType<TerrainGeneration>();
        treePlacement = FindObjectOfType<TreePlacement>();
        dinoPlacement = FindObjectOfType<DinosaurPlacement>();
    }
    
    private void Start()
    {
        dinoViewerPresenter = new DinoViewerPresenter(dinoViewerVisualElement, this);
        terrainGenerationData = TerrainGenerationData.Load(terrainSaveName);
        treePlacementData = TreePlacementData.Load(treesSaveName);
        dinoPlacementData = DinosaurPlacementData.Load(dinoSaveName);
        targetManager = FindObjectOfType<TargetManager>();
        worldGeneratorPresenter = new WorldGeneratorPresenter(worldGeneratorVisualElement, this);

        Time.timeScale = 0;
    }
    
    
    
    private void Update()
    {
        if (meshGenerated && treesPlaced && dinosPlaced && !gameStarted)
        {
            Time.timeScale = 1;
            stegosaurusContainer = GameObject.FindGameObjectWithTag("StegoContainer");
            velociraptorContainer = GameObject.FindGameObjectWithTag("RaptorContainer");
            
            Invoke("InitDinoViewer", 0.5f);
            worldGeneratorVisualElement.Display(false);
            dinoViewerVisualElement.Display(true);
            
            gameStarted = true;
        }
        
        if (currentDino != null)
        {
            dinoViewerPresenter.hungerSlider.value = currentDino.hunger;
            dinoViewerPresenter.thirstSlider.value = currentDino.thirst;
            dinoViewerPresenter.energySlider.value = currentDino.energy;
            dinoViewerPresenter.currentAction.text = "Current Action: " + currentDino.currentAction;
        }
    }

    private void InitDinoViewer()
    {
        
        foreach (var VARIABLE in targetManager.carnivoreFood)
        {
            Debug.Log("Carnivore Food - "+ VARIABLE.Key);
        }

        foreach (var VARIABLE in targetManager.herbivoreFood)
        {
            Debug.Log("Herbivore Food - "+ VARIABLE.Key);
        }
        
        //Init
        FindDinosInScene();
        dinoViewerPresenter.stegosaurus = stegosaurus;
        dinoViewerPresenter.velociraptor = velociraptor;
        dinoViewerPresenter.cameraOrbit = cameraOrbit;
        dinoViewerPresenter.SetDinoType("Stegosaurus");

        dinoViewerPresenter.LoadDinoStats(0);
        dinoViewerPresenter.seedNumber.text = "Seed: " + terrainGenerationData.seed;
    }
    
    private void FindDinosInScene()
    {
        if (stegosaurusContainer != null)
        {
            if (stegosaurusContainer.transform.childCount > 0)
            {
                stegosaurus = new List<GameObject>();
            
                foreach (Transform child in stegosaurusContainer.transform)
                {
                    stegosaurus.Add(child.gameObject);
                }
            }
        }

        if (velociraptorContainer != null)
        {
            if (velociraptorContainer.transform.childCount > 0)
            {
                velociraptor = new List<GameObject>();

                foreach (Transform child in velociraptorContainer.transform)
                {
                    velociraptor.Add(child.gameObject);
                } 
            }
        }
    }
}
