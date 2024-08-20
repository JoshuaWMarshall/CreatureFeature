using System;
using System.Drawing.Imaging;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldGeneratorPresenter : MonoBehaviour
{
    private GameManager _gameManager;
    // World Gen Window
    private Button openCloseButton;
    public Action openCloseAction { set => openCloseButton.clicked += value; }
    private VisualElement worldGeneratorVisualElement;
    
    // Window Selector Buttons
    private VisualElement buttonContainer;
    private Button terrainGenButton;
    private Button treePlacementButton;
    private Button dinoPlacementButton;

    public Action terrainGenDisplayAction { set => terrainGenButton.clicked += value; }
    public Action treePlacementDisplayAction { set => treePlacementButton.clicked += value; }
    public Action dinoPlacementDisplayAction { set => dinoPlacementButton.clicked += value; }
    
    //Terrain Gen
    private TerrainGeneration terrainGeneration;
    private int newTgSeed;
    
    private ScrollView svTerrainGen;
    private Label tgTitle;
    private Toggle tgRandomiseSeed;
    private IntegerField tgSeed;
    private Slider tgNoiseScale;
    private Slider tgLacunarity;
    private SliderInt tgOctaves;
    private SliderInt meshXSize;
    private SliderInt meshZSize;
    private SliderInt meshScale;
    private Slider heightMultiplier;
    private SliderInt waterHeight;
    private Button generateTerrain;
    
    public Action generateTerrainAction { set => generateTerrain.clicked += value; }
    
    // Tree Placement
    private TreePlacement treePlacement;
    private int newTpSeed;
    
    private ScrollView svTreePlacement;
    private Label tpTitle;
    private Slider tpNoiseScale;
    private Slider tpLacunarity;
    private SliderInt tpOctaves;
    private Slider intensity;
    private Slider randomness;
    private SliderInt maxSteepness;
    private Button placeTrees;
    
    public Action placeTreesAction { set => placeTrees.clicked += value; }
    
    // Dino Placement
    private DinosaurPlacement dinosaurPlacement;
    
    private Label dpTitle;
    private ScrollView svDinoPlacement;
    private SliderInt maxStegos;
    private SliderInt maxRaptors;
    private Button placeDinos;
    
    public Action placeDinosAction { set => placeDinos.clicked += value; }

    public WorldGeneratorPresenter(VisualElement root, GameManager gameManager)
    {
        this._gameManager = gameManager;
        terrainGeneration = gameManager.terrainGeneration;
        treePlacement =  gameManager.treePlacement;
        dinosaurPlacement = this._gameManager.dinoPlacement;

        worldGeneratorVisualElement = root.Q<VisualElement>("WorldGenPanel");
        openCloseButton = root.Q<Button>("OpenClose");

        openCloseAction = () => ToggleWorldGeneratorr();
        
        buttonContainer = root.Q <VisualElement>("ButtonContainer");
        terrainGenButton = buttonContainer.Q<Button>("Button_TerrainGen");
        treePlacementButton = buttonContainer.Q<Button>("Button_TreePlacement");
        dinoPlacementButton = buttonContainer.Q<Button>("Button_DinoPlacement");
       
        terrainGenDisplayAction = () => DisplayScrollView(svTerrainGen);
        treePlacementDisplayAction = () => DisplayScrollView(svTreePlacement);
        dinoPlacementDisplayAction = () => DisplayScrollView(svDinoPlacement);
        
        // Terrain Gen
        svTerrainGen = root.Q<ScrollView>("ScrollView_TerrainGen");
        tgTitle = svTerrainGen.Q<Label>("Label_TerrainGenTitle");
        tgRandomiseSeed = svTerrainGen.Q<Toggle>("Toggle_RandomiseSeed");
        tgSeed =  svTerrainGen.Q<IntegerField>("IntegerField_Seed");
        tgNoiseScale = svTerrainGen.Q<Slider>("Slider_NoiseScale");
        tgLacunarity = svTerrainGen.Q<Slider>("Slider_Lacunarity");
        tgOctaves = svTerrainGen.Q<SliderInt>("SliderInt_Octaves");
        meshXSize = svTerrainGen.Q<SliderInt>("SliderInt_MeshXSize");
        meshZSize = svTerrainGen.Q<SliderInt>("SliderInt_MeshZSize");
        meshScale = svTerrainGen.Q<SliderInt>("SliderInt_MeshScale");
        heightMultiplier = svTerrainGen.Q<Slider>("Slider_HeightMultiplier"); 
        waterHeight = svTerrainGen.Q<SliderInt>("SliderInt_WaterHeight");
        generateTerrain = svTerrainGen.Q<Button>("Button_GenerateTerrain");


        tgRandomiseSeed.RegisterCallback<MouseDownEvent>((evt) => {ToggleRandomiseSeed();}, TrickleDown.TrickleDown);
        tgSeed.RegisterValueChangedCallback(value => SetSeed(value.newValue));
        tgNoiseScale.RegisterValueChangedCallback(value => SetTgNoiseScale(value.newValue));
        tgLacunarity.RegisterValueChangedCallback(value => SetTgLacunarity(value.newValue));
        tgOctaves.RegisterValueChangedCallback(value => SetTgOctaves(value.newValue));
        meshXSize.RegisterValueChangedCallback(value => SetXSize(value.newValue));
        meshZSize.RegisterValueChangedCallback(value => SetZSize(value.newValue));
        meshScale.RegisterValueChangedCallback(value => SetMeshSchale(value.newValue));
        heightMultiplier.RegisterValueChangedCallback(value => SetHeightMultiplier(value.newValue));
        waterHeight.RegisterValueChangedCallback(value => SetWaterHeight(value.newValue));
        generateTerrainAction = GenerateTerrain;
        
        InitTerrainGen();
        
        // Tree Placement
        svTreePlacement = root.Q<ScrollView>("ScrollView_TreePlacement");
        tpTitle = svTreePlacement.Q<Label>("Label_TreePlacementTitle");
        tpNoiseScale = svTreePlacement.Q<Slider>("Slider_NoiseScale");
        tpLacunarity = svTreePlacement.Q<Slider>("Slider_Lacunarity");
        tpOctaves = svTreePlacement.Q<SliderInt>("SliderInt_Octaves");
        intensity = svTreePlacement.Q<Slider>("Slider_Intensity");
        randomness = svTreePlacement.Q<Slider>("Slider_Randomness");
        maxSteepness = svTreePlacement.Q<SliderInt>("SliderInt_MaxSteepness");
        placeTrees = svTreePlacement.Q<Button>("Button_PlaceTrees");
        
        tpNoiseScale.RegisterValueChangedCallback(value => SetTpNoiseScale(value.newValue));
        tpLacunarity.RegisterValueChangedCallback(value => SetTpLacunarity(value.newValue));
        tpOctaves.RegisterValueChangedCallback(value => SetTpOctaves(value.newValue));
        intensity.RegisterValueChangedCallback(value => SetIntensity(value.newValue));
        randomness.RegisterValueChangedCallback(value => SetRandomness(value.newValue));
        maxSteepness.RegisterValueChangedCallback(value => SetMaxSteepness(value.newValue));
        placeTreesAction = PlaceTrees;
        
        InitTreePlacement();
        
        // Dino Placement
        svDinoPlacement = root.Q<ScrollView>("ScrollView_DinosaurPlacement");
        dpTitle = svDinoPlacement.Q<Label>("Label_DinoPlacementTitle");
        maxStegos = svDinoPlacement.Q<SliderInt>("SliderInt_MaxStegos");
        maxRaptors = svDinoPlacement.Q<SliderInt>("SliderInt_MaxRaptors");
        placeDinos = svDinoPlacement.Q<Button>("Button_PlaceDinos");
        
        maxStegos.RegisterValueChangedCallback(value => SetMaxStegos(value.newValue));
        maxRaptors.RegisterValueChangedCallback(value => SetMaxRaptors(value.newValue));
        placeDinosAction = PlaceDinos;
        
        InitDinoPlacement();;
    }
    
    #region Terrain Gen

    private void InitTerrainGen()
    {
        tgRandomiseSeed.value = _gameManager.terrainGenerationData.randomiseSeed;
        tgSeed.value = _gameManager.terrainGenerationData.seed;
        tgNoiseScale.value = _gameManager.terrainGenerationData.noiseScale;
        tgLacunarity.value = _gameManager.terrainGenerationData.lacunarity;
        tgOctaves.value = _gameManager.terrainGenerationData.octaves;
        meshXSize.value = _gameManager.terrainGenerationData.xSize;
        meshZSize.value = _gameManager.terrainGenerationData.zSize;
        meshScale.value = _gameManager.terrainGenerationData.meshScale;
        heightMultiplier.value = _gameManager.terrainGenerationData.heightMultiplier;
        waterHeight.value = (int)_gameManager.terrainGenerationData.waterHeight;
    }

    private void ToggleRandomiseSeed()
    {
        _gameManager.terrainGenerationData.randomiseSeed = tgRandomiseSeed.value;
    }

    private void SetSeed(int value)
    {
        _gameManager.terrainGenerationData.seed = value;
    }

    private void SetTgNoiseScale(float value)
    {
        _gameManager.terrainGenerationData.noiseScale = value;
    }
    
    private void SetTgLacunarity(float value)
    {
        _gameManager.terrainGenerationData.lacunarity = value;
    }
    
    private void SetTgOctaves(int value)
    {
        _gameManager.terrainGenerationData.octaves = value;
    }

    private void SetXSize(int value)
    {
        _gameManager.terrainGenerationData.xSize = value;
    }
    
    private void SetZSize(int value)
    {
        _gameManager.terrainGenerationData.zSize = value;
    }

    private void SetMeshSchale(int value)
    {
        _gameManager.terrainGenerationData.meshScale = value;
    }

    private void SetHeightMultiplier(float value)
    {
        _gameManager.terrainGenerationData.heightMultiplier = value;
    }

    private void SetWaterHeight(int value)
    {
        _gameManager.terrainGenerationData.waterHeight = value;
    }

    private void GenerateTerrain()
    {
        terrainGeneration.DestroyTerrain(_gameManager.terrainGenerationData, _gameManager);
        
        // make noise texture
        int width = (int)(_gameManager.terrainGenerationData.xSize);
        int height = (int)(_gameManager.terrainGenerationData.zSize);
        _gameManager.terrainGenerationData.noiseMap = Noise.GetNoiseMap(width, height, _gameManager.terrainGenerationData.noiseScale, _gameManager.terrainGenerationData.octaves,
            _gameManager.terrainGenerationData.lacunarity, _gameManager.terrainGenerationData.seed, _gameManager.terrainGenerationData.randomiseSeed, out newTgSeed);
        _gameManager.terrainGenerationData.seed = newTgSeed;
        
        // generate terrain
        terrainGeneration.GenerateTerrain(_gameManager.terrainGenerationData, _gameManager);
        // generate water mesh
        terrainGeneration.CreateWaterMesh(_gameManager.terrainGenerationData);
    } 
    
    #endregion
    
    #region Tree Placement
    private void InitTreePlacement()
    {
        tpNoiseScale.value = _gameManager.treePlacementData.noiseScale;
        tpLacunarity.value = _gameManager.treePlacementData.lacunarity;
        tpOctaves.value = _gameManager.treePlacementData.octaves;
        intensity.value = _gameManager.treePlacementData.intensity;
        randomness.value = _gameManager.treePlacementData.randomness;
        maxSteepness.value = (int)_gameManager.treePlacementData.maxSteepness;
    }
    
    private void SetTpNoiseScale(float value)
    {
        _gameManager.treePlacementData.noiseScale = value;
    }
    
    private void SetTpLacunarity(float value)
    {
        _gameManager.treePlacementData.lacunarity = value;
    }
    
    private void SetTpOctaves(int value)
    {
        _gameManager.treePlacementData.octaves = value;
    }

    private void SetIntensity(float value)
    {
        _gameManager.treePlacementData.intensity = value;
    }

    private void SetRandomness(float value)
    {
        _gameManager.treePlacementData.randomness = value;
    }

    private void SetMaxSteepness(float value)
    {
        _gameManager.treePlacementData.maxSteepness = value;
    }

    private void PlaceTrees()
    {
        treePlacement.ClearTrees(_gameManager.treePlacementData, _gameManager);
        
        int width = (int)_gameManager.terrainGenerationData.xSize;
        int height = (int)_gameManager.terrainGenerationData.zSize;
        _gameManager.treePlacementData.noiseMap = Noise.GetNoiseMap(width, height, _gameManager.treePlacementData.noiseScale, _gameManager.treePlacementData.octaves, _gameManager.treePlacementData.lacunarity,
            _gameManager.terrainGenerationData.seed, _gameManager.terrainGenerationData.randomiseSeed, out newTpSeed);
        
        treePlacement.PlaceTrees(_gameManager.treePlacementData, _gameManager.terrainGeneration,
            _gameManager.terrainGenerationData, _gameManager);
    }
    #endregion
    
    #region Dino Placement

    private void InitDinoPlacement()
    {
        maxStegos.value = _gameManager.dinoPlacementData.maxStegosaurus;
        maxRaptors.value = _gameManager.dinoPlacementData.maxVelociraptors;
    }
    
    private void SetMaxStegos(int value)
    {
        _gameManager.dinoPlacementData.maxStegosaurus = value;
    }
    
    private void SetMaxRaptors(int value)
    {
        _gameManager.dinoPlacementData.maxVelociraptors = value;
    }

    private void PlaceDinos()
    {
        dinosaurPlacement.ClearDinos(_gameManager.dinoPlacementData, _gameManager);
        dinosaurPlacement.PlaceDinos(_gameManager.terrainGeneration, _gameManager.terrainGenerationData, _gameManager.dinoPlacementData, _gameManager);
    }
    #endregion 
    
    private void ToggleWorldGeneratorr()
    {
        bool active = (worldGeneratorVisualElement.style.left == 0);

        if (active)
        {
            worldGeneratorVisualElement.style.left = -455;
        }
        else
        {
            worldGeneratorVisualElement.style.left = 0;
        }
    }
    
    private void DisplayScrollView(ScrollView scrollView)
    {
        svTerrainGen.Display(false);
        svTreePlacement.Display(false);
        svDinoPlacement.Display(false);
        
        scrollView.Display(true);
    }
}
