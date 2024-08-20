using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
public class DinoViewerPresenter 
{
    private GameManager _gameManager;
    public List<GameObject> displayedDinos = new List<GameObject>();
    public List<GameObject> stegosaurus = new List<GameObject>();
    public List<GameObject> velociraptor = new List<GameObject>();

    private int currentIndex = 0;
    public CameraOrbit cameraOrbit;
    
    //Panel
    private VisualElement contents;

    //Text
    public Label seedNumber;
    public Label currentAction;
    private Label dinoName;

    //Drop down
    private DropdownField dinoTypeSelect;

    private List<string> dinoTypes = new List<string>()
    {
        "Stegosaurus",
        "Velociraptor"
    };

    //Buttons
    private Button openClosButton;
    private Button previousButton;
    private Button nextButton;

    public Action OpenCloseAction { set => openClosButton.clicked += value; }
    public Action PreviousDinoAction { set => previousButton.clicked += value; }
    public Action NextDinoAction { set => nextButton.clicked += value; }

    //Sliders
    public Slider hungerSlider;
    public Slider thirstSlider;
    public Slider energySlider;

    private VisualElement hungerSliderDragger;
    private VisualElement thirstSliderDragger;
    private VisualElement energySliderDragger;

    private VisualElement hungerSliderFillBar;
    private VisualElement thirstSliderFillBar;
    private VisualElement energySliderFillBar;

    public DinoViewerPresenter(VisualElement root, GameManager gameManager)
    {
        contents = root.Q<VisualElement>("Contents");
        seedNumber = root.Q<Label>("Label_SeedNum");
        currentAction = root.Q<Label>("Label_CurrentAction");
        dinoName = root.Q<Label>("Label_DinoName");
        
        openClosButton = root.Q<Button>("Button_OpenClose");
        previousButton = root.Q<Button>("Button_Previous");
        nextButton = root.Q<Button>("Button_Next");
        
        dinoTypeSelect = root.Q<DropdownField>("Dropdown_DinoTypeSelect");
        
        hungerSlider = root.Q<Slider>("Slider_Hunger");
        thirstSlider = root.Q<Slider>("Slider_Thirst");
        energySlider = root.Q<Slider>("Slider_Energy");

        hungerSliderDragger = hungerSlider.Q<VisualElement>("unity-dragger");
        thirstSliderDragger = thirstSlider.Q<VisualElement>("unity-dragger");
        energySliderDragger = energySlider.Q<VisualElement>("unity-dragger");
        
        AddFillBars();
        
        dinoTypeSelect.choices = dinoTypes;
        dinoTypeSelect.index = 0;
        dinoTypeSelect.RegisterValueChangedCallback((value) => SetDinoType(value.newValue));


        OpenCloseAction = () => ToggleDinoViewer();
        PreviousDinoAction = () => LoadDinoStats(-1);
        NextDinoAction = () => LoadDinoStats(1);

        //hungerSlider.RegisterValueChangedCallback((value) => SetDinoHunger(value.newValue));
        //thirstSlider.RegisterValueChangedCallback((value) => SetDinoThirst(value.newValue));
        //energySlider.RegisterValueChangedCallback((value) => SetDinoEnergy(value.newValue));

        this._gameManager = gameManager;
    }
    
    private void ToggleDinoViewer()
    {
        bool enabled = contents.IsDisplayFlex();

        contents.Display(!enabled);
    }
    
    public void SetDinoType(string type)
    {
        switch (type)
        {
            case "Stegosaurus":
                displayedDinos = stegosaurus;
                break;
            case "Velociraptor":
                displayedDinos = velociraptor;
                break;
        }
        ResetList();
        LoadDinoStats(0);
    }

    private void AddFillBars()
    {
        hungerSliderFillBar = new VisualElement();
        hungerSliderDragger.Add(hungerSliderFillBar);
        hungerSliderFillBar.name = "FillBar";
        hungerSliderFillBar.AddToClassList("fillBar");
        
        thirstSliderFillBar = new VisualElement();
        thirstSliderDragger.Add(thirstSliderFillBar);
        thirstSliderFillBar.name = "FillBar";
        thirstSliderFillBar.AddToClassList("fillBar");
        
        energySliderFillBar = new VisualElement();
        energySliderDragger.Add(energySliderFillBar);
        energySliderFillBar.name = "FillBar";
        energySliderFillBar.AddToClassList("fillBar");
    }
    
    public void LoadDinoStats(int indentBy)
    {
        Indent(indentBy);
        GAgent currentDino = CurrentDino();
        
        if (currentDino!)
        {
            _gameManager.currentDino = currentDino;

            cameraOrbit.target = displayedDinos[currentIndex];
            cameraOrbit.UpdateCameraPosition();
            currentAction.text = "Current Action: " + currentDino.currentAction;
            dinoName.text = "Dino Name :" + currentDino.name;
            hungerSlider.value = (float)currentDino.hunger;
            thirstSlider.value = (float)currentDino.thirst;
            energySlider.value = (float)currentDino.energy;
        }
    }

    public GAgent CurrentDino()
    {
        if (displayedDinos == null || displayedDinos.Count == 0)
        {
            return null;
        }
        return displayedDinos[currentIndex].GetComponent<GAgent>();
    }

    public void Indent(int step)
    {
        if (displayedDinos == null || displayedDinos.Count == 0)
        {
            return;
        }

        currentIndex = (currentIndex + step + displayedDinos.Count) % displayedDinos.Count;
    }

    public void ResetList()
    {
        currentIndex = 0;
    }
}
