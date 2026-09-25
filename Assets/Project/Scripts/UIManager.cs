using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private VisualElement qteContainer;

    private Label key1;
    private Label key2;
    private Label key3;

    private void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        qteContainer = root.Q<VisualElement>("qte-container");

        key1 = root.Q<Label>("key-1");
        key2 = root.Q<Label>("key-2");
        key3 = root.Q<Label>("key-3");

        CacherUI();
    }

    public void AfficherUI(string touche1, string touche2, string touche3)
    {
        qteContainer.style.display = DisplayStyle.Flex;

        key1.text = touche1;
        key2.text = touche2;
        key3.text = touche3;

        key1.style.display = DisplayStyle.Flex;
        key2.style.display = DisplayStyle.Flex;
        key3.style.display = DisplayStyle.Flex;
    }

    public void CacherUI()
    {
       qteContainer.style.display = DisplayStyle.None;
    }

    public void CacherTouche(int etape)
    {
        switch (etape)
        {
            case 0:
                key1.style.display = DisplayStyle.None;
                break;

            case 1:
                key2.style.display = DisplayStyle.None;
                break;

            case 2:
                key3.style.display = DisplayStyle.None;
                break;
        }
    }
}
