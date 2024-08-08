using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class DinoViewerPresenter : MonoBehaviour
{
    private int currentIndex = 0;
    private GameObject stegosaurusContainer;
    private GameObject velociraptorContainer;
    private GameObject[] stegosaurus;
    private GameObject[] velociraptor;

    private GameObject[] displayedDinos;
    private GAgent currentDino;

    //Panel
    [SerializeField] public UIDocument uIDocument = null;
    private VisualElement root;
    private VisualElement contents;

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
    public Action PreviousDinoAction { set => previousButton.clicked += value; }
    public Action NextDinoAction { set => nextButton.clicked += value; }

    //Sliders
    private Slider hungerSlider;
    private Slider thirstSlider;
    private Slider energySlider;

    public DinoViewerPresenter()
    {
        contents = root.Q<VisualElement>("Contents");
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

        dinoTypeSelect.choices = dinoTypes;
        dinoTypeSelect.RegisterValueChangedCallback((value) => SetDinoType(value.newValue));
        dinoTypeSelect.index = 0;

        OpenCloseAction = () => ToggleDinoViewer();
        PreviousDinoAction = () => LoadDinoStats(-1);
        NextDinoAction = () => LoadDinoStats(1);

        hungerSlider.RegisterValueChangedCallback((value) => SetDinoHunger(value.newValue));
        thirstSlider.RegisterValueChangedCallback((value) => SetDinoThirst(value.newValue));
        energySlider.RegisterValueChangedCallback((value) => SetDinoEnergy(value.newValue));

        title.text = "Dino Viewer 3000 ";
        dinoName.text = "Stego-doyouthinhkhesawus";

        //stegosaurusContainer = stegoContainer;
        //velociraptorContainer = raptorContainer;

        FindDinosInScene();
        displayedDinos = stegosaurus;
        LoadDinoStats(0);

    }

    private void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
    }

    private void LateUpdate()
    {
        if ((currentDino != null))
        {
            SetDinoEnergy(currentDino.energy);
            SetDinoHunger(currentDino.hunger);
            SetDinoThirst(currentDino.thirst);
        }
    }

    private void FindDinosInScene()
    {
        stegosaurus = new GameObject[stegosaurusContainer.transform.childCount];
        velociraptor = new GameObject[velociraptorContainer.transform.childCount];

        int i = 0;

        foreach (Transform child in stegosaurusContainer.transform)
        {
            stegosaurus[i] = child.gameObject;
            i += 1;
        }

        i = 0;

        foreach (Transform child in velociraptorContainer.transform)
        {
            velociraptor[i] = child.gameObject;
            i += 1;
        }
    }

    private void ToggleDinoViewer()
    {
        bool enabled = contents.IsDisplayFlex();

        contents.Display(!enabled);
    }


    private void SetDinoType(string type)
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

        LoadDinoStats(0);
    }

    private void SetDinoHunger(float hunger)
    {

    }

    private void SetDinoThirst(float hunger)
    {

    }

    private void SetDinoEnergy(float hunger)
    {

    }

    private void LoadDinoStats(int indentBy)
    {
        if (displayedDinos == null)
        {
            return;
        }

        if (displayedDinos.Length + indentBy < 0 || displayedDinos.Length + indentBy > displayedDinos.Length)
        {
            return;
        }

        currentDino = displayedDinos[currentIndex + indentBy].GetComponent<GAgent>();

        hungerSlider.value = (float)currentDino.hunger;
        thirstSlider.value = (float)currentDino.thirst;
        energySlider.value = (float)currentDino.energy;
        
    }
}
