using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    private Character _player;
    private AugmentManager _augmentManager;

    [Header("Input")]
    [SerializeField] private InputActionAsset InputActions;

    private InputActionMap _playerMap;
    private InputActionMap _uiMap;
    private InputAction m_shopAction;
    private InputAction m_shopUIAction;

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
    private VisualElement _experienceBar;

    [Header("AugmentShop")]
    [SerializeField] private AugmentData[] _availableAugments;
    private VisualElement _shopContainer;
    private Label[] _augmentNames = new Label[3];
    private Label[] _augmentDescriptions = new Label[3];
    private Label[] _augmentRarities = new Label[3];
    private Button _rerollButton;
    private Label _skillPointsLabel;
    private AugmentData[] _currentAugments = new AugmentData[3];
    [SerializeField] private int _rerollCost = 1;

    [Header("Spells")]
    private VisualElement[] _spellCooldownOverlays;
    private Label[] _spellCooldownTexts;
    private Label[] _spellChargeLabels;

    private void Awake()
    {
        _playerMap = InputActions.FindActionMap("Player");
        _uiMap = InputActions.FindActionMap("UI");

        m_shopAction = _playerMap.FindAction("shop");
        m_shopUIAction = _uiMap.FindAction("shop");

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

        _spellChargeLabels = new Label[]
        {
            root.Q<Label>("spell_charges-text-1"),
            root.Q<Label>("spell_charges-text-2"),
            root.Q<Label>("spell_charges-text-3"),
            root.Q<Label>("spell_charges-text-4"),
        };

        _experienceBar = root.Q<VisualElement>("experience-bar");
        _experienceBar.style.width = Length.Percent(0f);

        _shopContainer = root.Q<VisualElement>("augment-shop");
        _shopContainer.style.display = DisplayStyle.None;

        for (int i = 0; i < 3; i++)
        {
            int number = i + 1;

            _augmentNames[i] =
                root.Q<Label>($"augment-name-{number}");

            _augmentDescriptions[i] =
                root.Q<Label>($"augment-description-{number}");

            _augmentRarities[i] =
                root.Q<Label>($"augment-rarity-{number}");
        }

        for (int i = 0; i < 3; i++)
        {
            int index = i;
            var card = root.Q<VisualElement>($"augment-card-{index + 1}");

            if (card == null)
            {
                Debug.LogError($"Carte augment-card-{index + 1} introuvable.");
                continue;
            }

            card.RegisterCallback<ClickEvent>(_ => AcheterAugment(index));
        }

        _rerollButton = root.Q<Button>("reroll-button");
        _skillPointsLabel = root.Q<Label>("skill-points-label");

        _rerollButton.clicked += AcheterReroll;

        CacherUI();
    }

    public void SetPlayer(Character player)
    {
        _player = player;
        _augmentManager = player.GetComponent<AugmentManager>();

        if (_augmentManager == null)
            Debug.LogError("AugmentManager introuvable sur le player");

        UpdateVie();
        UpdateRage();
        UpdateExperience();
        ActualiserSkillPoints();
    }

    private void Start()
    {
        RerollAugments();
        ActualiserSkillPoints();
    }

    private void Update()
    {
        if (_player == null)
            return;

        UpdateVie();
        UpdateRage();
        UpdateExperience();
        ActualiserSkillPoints();

        if (m_shopAction.WasPressedThisFrame())
        {
            _shopContainer.style.display = DisplayStyle.Flex;

            _playerMap.Disable();
            _uiMap.Enable();

            Time.timeScale = 0f;
        }

        if (m_shopUIAction.WasPressedThisFrame())
        {
            _shopContainer.style.display = DisplayStyle.None;

            _playerMap.Enable();
            _uiMap.Disable();

            Time.timeScale = 1f;
        }
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

    public void UpdateSpellCharges(int index, int charges, int maxCharges)
    {
        if (index < 0 || index >= _spellChargeLabels.Length)
            return;

        if (maxCharges <= 1)
        {
            _spellChargeLabels[index].style.display = DisplayStyle.None;
            return;
        }

        _spellChargeLabels[index].style.display = DisplayStyle.Flex;
        _spellChargeLabels[index].text = charges + " / " + maxCharges;
    }

    private void RerollAugments()
    {
        var groupes = new Dictionary<AugmentRarity, List<AugmentData>>();

        foreach (AugmentData augment in _availableAugments)
        {
            if (!groupes.ContainsKey(augment._rarity))
            {
                groupes[augment._rarity] = new List<AugmentData>();
            }

            groupes[augment._rarity].Add(augment);
        }

        var raretesPossibles = new List<AugmentRarity>();

        foreach (var groupe in groupes)
        {
            if (groupe.Value.Count >= 3)
                raretesPossibles.Add(groupe.Key);
        }

        AugmentRarity rareteChoisie = raretesPossibles[UnityEngine.Random.Range(0, raretesPossibles.Count)];

        var disponibles = new List<AugmentData>(groupes[rareteChoisie]);

        for (int i = 0; i < 3;  i++)
        {
            int index = UnityEngine.Random.Range(0, disponibles.Count);
            AugmentData augment = disponibles[index];
            disponibles.RemoveAt(index);

            _currentAugments[i] = augment;

            _augmentNames[i].text = augment._augmentName;
            _augmentDescriptions[i].text = augment._description;
            _augmentRarities[i].text = augment._rarity.ToString();
        }
    }

    private void AcheterAugment(int index)
    {
        Debug.Log($"Clic détecté sur la carte {index}");

        if (index < 0 || index >= _currentAugments.Length)
            return;

        if (_player == null || _augmentManager == null)
            return;

        AugmentData augment = _currentAugments[index];

        if (augment == null || augment._effect == null)
        {
            Debug.LogWarning("Cet augment n'a pas encore d'effet assigné.");
            return;
        }

        if (_player.SkillPoints < augment.SkillPointCost)
        {
            Debug.Log("Pas assez de Skill Points.");
            return;
        }

        if (_augmentManager.AddAugment(augment))
        {
            _player.SpendSkillPoints(augment.SkillPointCost);
            ActualiserSkillPoints();
            RerollAugments();
        }
    }

    private void AcheterReroll()
    {
        if (_player == null)
            return;

        if (_player.SpendSkillPoints(_rerollCost))
        {
            ActualiserSkillPoints();
            RerollAugments();
        }
        else
        {
            Debug.Log("Pas assez de Skill Points pour le Reroll.");
        }
    }

    private void ActualiserSkillPoints()
    {
        if (_player == null || _skillPointsLabel == null)
            return;

        _skillPointsLabel.text = $"SKILL POINTS : {_player.SkillPoints}";
    }
}
