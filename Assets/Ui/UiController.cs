using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiController : MonoBehaviour
{
    private DinoViewerPresenter dinoViewerPresenter;
    public GameObject stegosaurusContainer;
    public GameObject velociraptorContainer;
    private void Start()
    {
        //VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        //dinoViewerPresenter = new DinoViewerPresenter(root, stegosaurusContainer, velociraptorContainer);
    }
}
