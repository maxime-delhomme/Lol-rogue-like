using System;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private QTEManager _qteManager;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private EnemySpawner _enemySpawner;

    public enum GameDifficulty
    {
        VeryEasy,
        Easy,
        Normal,
        Hard,
        VeryHard,
        Extreme
    }

    [Header("Difficulty")]
    [SerializeField] private GameDifficulty _currentDifficulty = GameDifficulty.VeryEasy;
    [SerializeField] private float _increaseDifficulty = 0f;
    [SerializeField] private float _difficultyThreshold = 100f;
    [SerializeField] private float _difficultyIncreasePerSecond = 1f;

    public GameDifficulty CurrentDifficulty => _currentDifficulty;



    private void Start()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        GameObject playerObject = Instantiate(_playerPrefab,new Vector3(0f, 1f, 0f),Quaternion.identity);

        Character player = playerObject.GetComponent<Character>();
        SpellCaster spellCaster = playerObject.GetComponent<SpellCaster>();

        _cameraController.SetTarget(playerObject.transform);

        _qteManager.SetPlayer(player);
        _qteManager.SetSpellCaster(spellCaster);

        spellCaster.SetQTEManager(_qteManager);
        spellCaster.SetUIManager(_uiManager);
        spellCaster.SetCamera(Camera.main);

        _uiManager.SetPlayer(player);

        _enemySpawner.SetPlayer(player);
    }

    private void Update()
    {
        _increaseDifficulty += _difficultyIncreasePerSecond * Time.deltaTime;

        if (_increaseDifficulty >= _difficultyThreshold)
        {
            IncreaseGameDifficulty();
        }
    }

    private void IncreaseGameDifficulty()
    {
        _increaseDifficulty -= _difficultyThreshold;

        if (_currentDifficulty >= GameDifficulty.Extreme)
        {
            _increaseDifficulty = 0f;
            return;
        }

        _currentDifficulty++;

        Debug.Log($"Difficulté actuelle : {_currentDifficulty}");
    }

    public void AddDifficultyProgress(float amount)
    {
        if (_currentDifficulty >= GameDifficulty.Extreme)
            return;

        _increaseDifficulty += amount;

        if (_increaseDifficulty >= _difficultyThreshold)
        {
            IncreaseGameDifficulty();
        }
    }

    public int GetDifficultyMultiplier()
    {
        return (int)_currentDifficulty + 1;
    }
}
