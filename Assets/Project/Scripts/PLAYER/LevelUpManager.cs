using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelUpManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private UIManager _uiManager;
    private Character _player;
    [Header("Input")]
    [SerializeField] private InputActionAsset _inputActions;

    [Header("Bonus")]
    [SerializeField] private LevelUpBonusData[] _bonusPool;

    private List<LevelUpBonusData> _currentBonus = new List<LevelUpBonusData>();

    private void Awake()
    {
        _player = GetComponent<Character>();
    }

    public void SetUIManager(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void StartLevelUp()
    {
        Time.timeScale = 0f;

        GenerateBonus();

        _inputActions.FindActionMap("Player").Disable();
        _inputActions.FindActionMap("UI").Enable();

        _uiManager.ShowLevelUp(_currentBonus.ToArray());
    }

    private void GenerateBonus()
    {
        _currentBonus.Clear();

        List<LevelUpBonusData> availableBonus = new List<LevelUpBonusData>(_bonusPool);

        for (int i = 0; i < 3; i++)
        {
            if (availableBonus.Count == 0)
                break;

            int randomIndex = Random.Range(0,availableBonus.Count);
            _currentBonus.Add(availableBonus[randomIndex]);
            availableBonus.RemoveAt(randomIndex);
        }
    }

    public void SelectBonus(int index)
    {
        if (index < 0 || index >= _currentBonus.Count)
            return;

        LevelUpBonusData selectedBonus = _currentBonus[index];

        _player.ApplyBonus(selectedBonus);
        _player.ConsumePendingLevelUp();

        _uiManager.HideLevelUp();

        if (_player.PendingLevelUps > 0)
        {
            GenerateBonus();

            _uiManager.ShowLevelUp(_currentBonus.ToArray());
            return;
        }

        _inputActions.FindActionMap("Player").Enable();
        _inputActions.FindActionMap("UI").Disable();

        Time.timeScale = 1f;
    }
}
