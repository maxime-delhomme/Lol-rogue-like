using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static EnemyData;

public class Autel : MonoBehaviour, IInteractable
{
    [Header("Reference")]
    [SerializeField] private Outline _outline;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private EnemySpawner _enemySpawner;

    [Header("Objective")]
    [SerializeField] private int _baseEnemiesToKill = 10;
    [SerializeField] private int _baseEnemiesPerWave = 3;
    private int _enemiesToKill;
    private int _enemiesPerWave;
    [SerializeField] private float _killRadius = 15f;
    [SerializeField] private GameObject _radiusVisual;
    private readonly List<Ennemi> _aliveEnemies = new List<Ennemi>();
    private bool _canCompleteObjective;
    [SerializeField] private float _difficultyAmount;

    [Header("Spawn")]
    [SerializeField] private EnemyData[] _enemyDataList;
    [SerializeField] private float _waveInterval = 10f;
    [SerializeField] private float _spawnInterval = 0.5f;
    [SerializeField] private float _spawnHeight = 2f;
    [SerializeField] private float _navMeshSampleDistance = 3f;
    [SerializeField] private float _commonWeight = 45f;
    [SerializeField] private float _uncommonWeight = 35f;
    [SerializeField] private float _rareWeight = 14f;
    [SerializeField] private float _epicWeight = 5f;
    [SerializeField] private float _legendaryWeight = 1f;

    private bool _isInteractive = true;
    private bool _isActivated;
    private bool _isCompleted;
    private int _enemiesKilled;
    public bool IsActivated => _isActivated;
    public bool IsCompleted => _isCompleted;
    public int EnemiesKilled => _enemiesKilled;
    public int EnemiesToKill => _enemiesToKill;



    private void Awake()
    {
        if (_radiusVisual != null)
        {
            _radiusVisual.SetActive(false);
        }

        _gameManager = FindAnyObjectByType<GameManager>();
        _enemySpawner = FindAnyObjectByType<EnemySpawner>();
    }

    private void Update()
    {

        if(_isCompleted == false)
        {
            if (_isInteractive == true)
            {
                _outline.OutlineColor = Color.white;
            }
            else
            {
                _outline.OutlineColor = Color.red;
            }
        }
    }

    private void ConfigureObjectiveDifficulty()
    {
        int difficultyMultiplier = _gameManager.GetDifficultyMultiplier();

        _enemiesToKill = _baseEnemiesToKill * difficultyMultiplier;
        _enemiesPerWave = _baseEnemiesPerWave * difficultyMultiplier;

        Debug.Log($"Objectif : {_enemiesToKill} ennemis à tuer, {_enemiesPerWave} par vague.");
    }

    public void Interact()
    {
        if (_isActivated == false && _isCompleted == false)
        {
            ConfigureObjectiveDifficulty();
            _enemySpawner.SetSpawnPaused(true);

            _isActivated = true;
            _isInteractive = false;

            if (_radiusVisual != null)
            {
                _radiusVisual.SetActive(true);
            }

            StartCoroutine(SpawnWavesCoroutine());
        }
        else if (_canCompleteObjective == true && _isCompleted == false)
        {
            _aliveEnemies.RemoveAll(enemy => enemy == null);

            if (_aliveEnemies.Count == 0)
            {
                ObjectivCompleted();
                _enemySpawner.SetSpawnPaused(false);
            }
        }   
    }

    private IEnumerator SpawnWavesCoroutine()
    {
        while (_isCompleted == false && _canCompleteObjective == false)
        {
            yield return StartCoroutine(SpawnEnemiesCoroutine());

            yield return new WaitForSeconds(_waveInterval);
        }
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        int spawnCount = 0;

        while (spawnCount < _enemiesPerWave && _isCompleted == false)
        {
            SpawnEnemy();
            spawnCount++;

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (_enemyDataList.Length > 0)
        {
            EnemyData data = GetRandomEnemyData();

            if (data != null && data._prefab != null)
            {
                if (TryGetSpawnPosition(out Vector3 spawnPosition))
                {
                    GameObject enemyObject = Instantiate(data._prefab, spawnPosition, Quaternion.identity);

                    Ennemi enemy = enemyObject.GetComponent<Ennemi>();

                    if (enemy != null)
                    {
                        enemy.Initialiser(data);
                        enemy.DefinirAutel(this);

                        _aliveEnemies.Add(enemy);
                    }
                }
            }
        }
    }

    private EnemyData GetRandomEnemyData()
    {
        float totalWeight = 0f;

        foreach (EnemyData data in _enemyDataList)
        {
            totalWeight += GetRarityWeight(data);
        }

        if (totalWeight <= 0f)
            return null;

        float randomValue = Random.Range(0f, totalWeight);

        foreach (EnemyData data in _enemyDataList)
        {
            randomValue -= GetRarityWeight(data);

            if (randomValue <= 0f)
                return data;
        }

        return _enemyDataList[_enemyDataList.Length - 1];
    }

    private float GetRarityWeight(EnemyData data)
    {
        switch (data._rarity)
        {
            case EnemyRarity.Common:
                return _commonWeight;

            case EnemyRarity.Uncommon:
                return _uncommonWeight;

            case EnemyRarity.Rare:
                return _rareWeight;

            case EnemyRarity.Epic:
                return _epicWeight;

            case EnemyRarity.Legendary:
                return _legendaryWeight;

            default:
                return 0f;
        }
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * _killRadius;

            Vector3 candidatePosition = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);

            if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, _navMeshSampleDistance, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(transform.position, hit.position);

                if (distance <= _killRadius)
                {
                    spawnPosition = hit.position + Vector3.up * _spawnHeight;
                    return true;
                }
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    public void EnemyKilled()
    {
        if (_isActivated == true && IsCompleted == false)
        {
            _enemiesKilled++;

            _aliveEnemies.RemoveAll(enemy => enemy == null);

            if (_enemiesKilled >= _enemiesToKill && _canCompleteObjective == false)
            {
                _canCompleteObjective = true;
                StopAllCoroutines();

                Debug.Log("quota atteint");
                _isInteractive = true;
            }
        }
    }

    private void ObjectivCompleted()
    {
        if (_isCompleted == false)
        {
            _isCompleted = true;

            StopAllCoroutines();

            _outline.OutlineColor = Color.green;
            _outline.enabled = true;

            if (_radiusVisual != null)
            {
                _radiusVisual.SetActive(false);
            }

            _gameManager.AddDifficultyProgress(_difficultyAmount);

            Debug.Log("Objectif terminé");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _killRadius);
    }
}
