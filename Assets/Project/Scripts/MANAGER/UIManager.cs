using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    private Character player;

    [Header("QTE")]
    private VisualElement qteContainer;
    private VisualElement[] _keyContainers;
    private VisualElement[] _timers;
    private Label[] _keys;

    [Header("PlayerStats")]
    private Label healthText;
    private Label rageText;
    private VisualElement healthBar;
    private VisualElement rageBar;

    [Header("Spells")]
    private VisualElement spellIcon;
    private VisualElement spellCooldownOverlay;
    private Label spellCooldownText;

    private void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.GetComponent<Character>();
        }

        qteContainer = root.Q<VisualElement>("qte-container");
        _keys = new Label[]
        {
            root.Q<Label>("key1"),
            root.Q<Label>("key2"),
            root.Q<Label>("key3")
        };

        _keyContainers = new VisualElement[]
        {
            root.Q<VisualElement>("key-container-1"),
            root.Q<VisualElement>("key-container-2"),
            root.Q<VisualElement>("key-container-3")
        };

        _timers = new VisualElement[]
        {
            root.Q<VisualElement>("timer1"),
            root.Q<VisualElement>("timer2"),
            root.Q<VisualElement>("timer3")
        };

        healthText = root.Q<Label>("health-text");
        rageText = root.Q<Label>("rage-text");
        healthBar = root.Q<VisualElement>("health-bar");
        rageBar = root.Q<VisualElement>("rage-bar");

        spellIcon = root.Q<VisualElement>("spell-icon");
        spellCooldownOverlay = root.Q<VisualElement>("spell-cooldown-overlay");
        spellCooldownText = root.Q<Label>("spell-cooldown-text");

        CacherUI();
    }

    private void Update()
    {
        if (player == null)
            return;

        UpdateVie();
        UpdateRage();
    }

    private void UpdateVie()
    {
        healthText.text = player.CurrentHealth + " / " + player.MaxHealth;

        float pourcentage = (float)player.CurrentHealth / player.MaxHealth;

        healthBar.style.width = Length.Percent(pourcentage * 100f);
    }

    private void UpdateRage()
    {
        rageText.text = player.CurrentRage + " / " + player.MaxRage;

        float pourcentage = (float)player.CurrentRage / player.MaxRage;

        rageBar.style.width = Length.Percent(pourcentage * 100f); 
    }

    public void UpdateCooldownSpell(float tempsRestant, float cooldownMax)
    {
        if (tempsRestant <= 0f)
        {
            spellCooldownOverlay.style.height = Length.Percent(0);
            spellCooldownText.text = "";
            return;
        }

        float pourcentage = tempsRestant / cooldownMax;

        spellCooldownOverlay.style.height = Length.Percent(pourcentage * 100f);

        spellCooldownText.text = tempsRestant.ToString("0.0");
    }

    public void AfficherUI(string touche1, string touche2, string touche3)
    {
        _keys[0].text = touche1;
        _keys[1].text = touche2;
        _keys[2].text = touche3;

        for (int i = 0; i < _keyContainers.Length; i++)
        {
            _keyContainers[i].style.display = DisplayStyle.None;
            _timers[i].style.height = Length.Percent(100f);
        }

        qteContainer.style.display = DisplayStyle.Flex;
    }

    public void CacherUI()
    {
       qteContainer.style.display = DisplayStyle.None;
    }

    public void CacherTouche(int index)
    {
        _keyContainers[index].style.display = DisplayStyle.None;
    }

    public void AfficherTouche(int index)
    {
        _keyContainers[index].style.display = DisplayStyle.Flex;
        _timers[index].style.height = Length.Percent(100f);
    }

    public void UpdateTimerTouche(int index, float tempsRestant, float tempsMax)
    {
        float pourcentage = tempsRestant / tempsMax;

        _timers[index].style.height = Length.Percent(pourcentage * 100f);
    }
}
