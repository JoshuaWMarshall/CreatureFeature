using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class DinoViewerPresenter : MonoBehaviour
{
    //Text
    private Label seedNumber;
    private Label title;
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
    public Action PreviousAction { set => previousButton.clicked += value; }
    public Action NextAction { set => nextButton.clicked += value; }
    
    //Sliders
    private Slider hungerSlider;
    private Slider thirstSlider;
    private Slider energySlider;

    public DinoViewerPresenter(VisualElement root)
    {
        seedNumber = root.Q<Label>("Label_SeedNum");
        title = root.Q<Label>("Label_DinoViewPanel");
        dinoName = root.Q<Label>("Label_DinoName");
        openClosButton = root.Q<Button>("Button_OpenClose");
        previousButton = root.Q<Button>("Button_Previous");
        nextButton = root.Q<Button>("Button_Next");
        dinoTypeSelect = root.Q<DropdownField>("Dropdown_DinoTypeSelect");
        hungerSlider = root.Q<Slider>("Slider_Hunger");
        thirstSlider = root.Q<Slider>("Slider_Thirst");
        energySlider = root.Q<Slider>("Slider_Energy");
        
        
        
        
    }

    private void ToggleDinoViewer()
    {
        
    }
}
