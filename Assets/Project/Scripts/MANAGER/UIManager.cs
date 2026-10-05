using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    private Character _player;
    private LevelUpManager _levelUpManager;

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

    [Header("LevelUp")]
    private VisualElement _bonusContainer;
    private Button[] _bonusButtons;
    private Label[] _bonusNames;
    private Label[] _bonusDescriptions;
    private Label[] _bonusRarities;
    private VisualElement _experienceBar;
    private Label _levelUpTitle;

    [Header("Spells")]
    private VisualElement[] _spellCooldownOverlays;
    private Label[] _spellCooldownTexts;

    private void Awake()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

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

        _spellCooldownOverlays = new VisualElement[]
        {
            root.Q<VisualElement>("spell-cooldown-overlay-1"),
            root.Q<VisualElement>("spell-cooldown-overlay-2"),
            root.Q<VisualElement>("spell-cooldown-overlay-3"),
            root.Q<VisualElement>("spell-cooldown-overlay-4")
        };

        _spellCooldownTexts = new Label[]
        {
            root.Q<Label>("spell-cooldown-text-1"),
            root.Q<Label>("spell-cooldown-text-2"),
            root.Q<Label>("spell-cooldown-text-3"),
            root.Q<Label>("spell-cooldown-text-4")
        };

        _bonusContainer = root.Q<VisualElement>("level-up-container");

        _bonusButtons = new Button[]
        {
            root.Q<Button>("bonus-button-1"),
            root.Q<Button>("bonus-button-2"),
            root.Q<Button>("bonus-button-3")
        };

        for (int i = 0; i < _bonusButtons.Length; i++)
        {
            int index = i;

            _bonusButtons[i].clicked += () =>
            {
                _levelUpManager.SelectBonus(index);
            };
        }

        _bonusNames = new Label[]
        {
            root.Q<Label>("bonus-name-1"),
            root.Q<Label>("bonus-name-2"),
            root.Q<Label>("bonus-name-3")
        };

        _bonusDescriptions = new Label[]
        {
            root.Q<Label>("bonus-description-1"),
            root.Q<Label>("bonus-description-2"),
            root.Q<Label>("bonus-description-3")
        };

        _bonusRarities = new Label[]
        {
            root.Q<Label>("bonus-rarity-1"),
            root.Q<Label>("bonus-rarity-2"),
            root.Q<Label>("bonus-rarity-3")
        };

        _experienceBar = root.Q<VisualElement>("experience-bar");

        _levelUpTitle = root.Q<Label>("level-up-title");


        CacherUI();
        HideLevelUp();
    }

    public void SetPlayer(Character player)
    {
        _player = player;
    }

    public void SetLevelUpManager(LevelUpManager levelUpManager)
    {
        _levelUpManager = levelUpManager;
    }

    private void Update()
    {
        if (_player == null)
            return;

        UpdateVie();
        UpdateRage();
        UpdateExperience();
    }

    private void UpdateVie()
    {
        healthText.text = _player.CurrentHealth + " / " + _player.MaxHealth;

        float pourcentage = (float)_player.CurrentHealth / _player.MaxHealth;

        healthBar.style.width = Length.Percent(pourcentage * 100f);
    }

    private void UpdateRage()
    {
        rageText.text = _player.CurrentRage + " / " + _player.MaxRage;

        float pourcentage = (float)_player.CurrentRage / _player.MaxRage;

        rageBar.style.width = Length.Percent(pourcentage * 100f); 
    }
    private void UpdateExperience()
    {
        float pourcentage = (float)_player.CurrentExperience / _player.ExperienceRequired;

        _experienceBar.style.width = Length.Percent(pourcentage * 100f);
    }

    public void UpdateCooldownSpell(int index, float tempsRestant, float cooldownMax)
    {
        if (tempsRestant <= 0f)
        {
            _spellCooldownOverlays[index].style.height = Length.Percent(0);
            _spellCooldownTexts[index].text = "";
            return;
        }

        float pourcentage = tempsRestant / cooldownMax;

        _spellCooldownOverlays[index].style.height = Length.Percent(pourcentage * 100f);

        _spellCooldownTexts[index].text = tempsRestant.ToString("0.0");
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

    public void ShowLevelUp(LevelUpBonusData[] bonus)
    {
        _levelUpTitle.text = "Level UP ! Niveau " + _player.CurrentLevel;

        for (int i = 0; i < _bonusButtons.Length; i++)
        {
            if (i >= bonus.Length)
            {
                _bonusButtons[i].style.display = DisplayStyle.None;
                continue;
            }

            _bonusButtons[i].style.display = DisplayStyle.Flex;

            _bonusRarities[i].text = bonus[i]._rarity.ToString();
            _bonusNames[i].text = " + " + bonus[i]._value + " " + bonus[i]._bonusName;
            _bonusDescriptions[i].text = bonus[i]._description;
        }

        _bonusContainer.style.display = DisplayStyle.Flex;
    }
    
    public void HideLevelUp()
    {
        _bonusContainer.style.display = DisplayStyle.None;
    }
}
