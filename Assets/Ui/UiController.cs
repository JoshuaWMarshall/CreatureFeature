using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiController : MonoBehaviour
{
    private DinoViewerPresenter dinoViewerPresenter;
    private VisualElement root;

    private GameObject stegosaurusContainer;
    private GameObject velociraptorContainer;
    private List<GameObject> stegosaurus;
    private List<GameObject> velociraptor;

    [HideInInspector] public GAgent currentDino;
    private CameraOrbit cameraOrbit;
    
    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
    }
    
    private void Awake()
    {
        stegosaurusContainer = GameObject.FindGameObjectWithTag("StegoContainer");
        velociraptorContainer = GameObject.FindGameObjectWithTag("RaptorContainer");
        cameraOrbit = FindObjectOfType<CameraOrbit>();

    }
    
    private void Start()
    {
        dinoViewerPresenter = new DinoViewerPresenter(root, this);
        
        // delayed so all dinos are finished spawning
        Invoke("InitDinoViewer", 1f);
    }

    private void Update()
    {
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
        //Init
        FindDinosInScene();
        dinoViewerPresenter.displayedDinos= stegosaurus;
        dinoViewerPresenter.stegosaurus = stegosaurus;
        dinoViewerPresenter.velociraptor = velociraptor;
        dinoViewerPresenter.cameraOrbit = cameraOrbit;

        cameraOrbit.target = dinoViewerPresenter.CurrentDino().gameObject;
        dinoViewerPresenter.LoadDinoStats(0);
    }
    
    private void FindDinosInScene()
    {
        if (stegosaurusContainer.transform.childCount > 0)
        {
            stegosaurus = new List<GameObject>();
            
            foreach (Transform child in stegosaurusContainer.transform)
            {
                stegosaurus.Add(child.gameObject);
            }
        }

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
