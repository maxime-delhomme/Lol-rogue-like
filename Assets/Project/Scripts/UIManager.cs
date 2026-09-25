using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private VisualElement qteContainer;

    private Label keyQ;
    private Label keyS;
    private Label keyD;

    private void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        qteContainer = root.Q<VisualElement>("qte-container");

        keyQ = root.Q<Label>("key-q");
        keyS = root.Q<Label>("key-s");
        keyD = root.Q<Label>("key-d");

        CacherUI();
    }

    public void AfficherUI()
    {
        qteContainer.style.display = DisplayStyle.Flex;
    }

    public void CacherUI()
    {
       qteContainer.style.display = DisplayStyle.None;
    }
}
