using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static EnemyData;

public class EnemySpawner : MonoBehaviour
{
    [Header("Reference")]
    private Character _player;
    [SerializeField] private GameManager _gameManager;

    [Header("Ennemi")]
    [SerializeField] private EnemyData[] _enemyDataList;
    [SerializeField] private int _commonWeight = 45;
    [SerializeField] private int _uncommonWeight = 35;
    [SerializeField] private int _rareWeight = 14;
    [SerializeField] private int _epicWeight = 5;
    [SerializeField] private int _legendaryWeight = 1;
    [SerializeField] private int _rarityWeightIncreasePerMinute = 1;

    [SerializeField] private int _maxEpicEnemies = 1;
    private int _currentEpicEnemyCount;

    [Header("Spawn Budget")]
    [SerializeField] private int _spawnBudgetIncreasePerMinute = 1;
    [SerializeField] private int _maxSpawnBudget = 20;
    [SerializeField] private int _budgetRechargeDelay = 10;
    private int _currentSpawnBudget;
    private int _elapsedMinutes;
    private float _elapsedTime;
    private float _budgetRechargeTimer;

    [Header("Spawn")]
    [SerializeField] private float _minSpawnDistance;
    [SerializeField] private int _maxSpawnAttemps;
    [SerializeField] private int _enemyCounts;
    [SerializeField] private float _respawnDelay;
    private int _currentEnemyCount;

    public void SetPlayer(Character player)
    {
        _player = player;
    }

    private void Start()
    {
        _currentSpawnBudget = _maxSpawnBudget;
        _elapsedMinutes = 0;
        _elapsedTime = 0;
        _budgetRechargeTimer = 0f;

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private void Update()
    {
        if (_player == null)
            return;

        _elapsedTime += Time.deltaTime;

        UpdateSpawnBudget();
        UpdateBudgetRecharge();
    }

    private void UpdateSpawnBudget()
    {
        int elapsedMinutes = Mathf.FloorToInt(_elapsedTime / 60f);

        if (elapsedMinutes > _elapsedMinutes)
        {
            int minutesPassed = elapsedMinutes - _elapsedMinutes;

            _maxSpawnBudget += minutesPassed * _spawnBudgetIncreasePerMinute;
            _elapsedMinutes = elapsedMinutes;
        }
    }

    private void UpdateBudgetRecharge()
    {
        if (_currentSpawnBudget > 0)
        {
            _budgetRechargeTimer = 0f;
            return;
        }

        _budgetRechargeTimer += Time.deltaTime;

        if (_budgetRechargeTimer >= _budgetRechargeDelay)
        {
            _currentSpawnBudget = _maxSpawnBudget;
            _budgetRechargeTimer = 0f;
        }
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        while (true)
        {
            if (_currentSpawnBudget > 0 && _currentEnemyCount < _enemyCounts)
            {
                SpawnEnemy();
            }

            yield return new WaitForSeconds(.2f);
        }
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        if (_player == null)
        {
            spawnPosition = Vector3.zero;
            return false;
        }

        NavMeshTriangulation navMesh = NavMesh.CalculateTriangulation();

        for (int i = 0; i < _maxSpawnAttemps; i++)
        {
            int triangleIndex = Random.Range(0, navMesh.indices.Length / 3);

            Vector3 vertexA = navMesh.vertices[navMesh.indices[triangleIndex * 3]];
            Vector3 vertexB = navMesh.vertices[navMesh.indices[triangleIndex * 3 + 1]];
            Vector3 vertexC = navMesh.vertices[navMesh.indices[triangleIndex * 3 + 2]];

            Vector3 candidatePosition = GetRandomPointInTriangle(vertexA, vertexB, vertexC);

            if (Vector3.Distance(candidatePosition, _player.transform.position) < _minSpawnDistance)
                continue;

            spawnPosition = candidatePosition;
            return true;
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private Vector3 GetRandomPointInTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        float randomA = Random.value;
        float randomB = Random.value;

        if (randomA + randomB > 1f)
        {
            randomA = 1f - randomA;
            randomB = 1f - randomB;
        }

        return pointA + (pointB - pointA) * randomA + (pointC - pointA) * randomB;
    }

    private EnemyData GetRandomEnemyData()
    {
        int totalWeight = 0;

        foreach (EnemyData data in _enemyDataList)
        {
            if (data == null || data._prefab == null)
                continue;

            if (_gameManager.CurrentDifficulty == GameManager.GameDifficulty.VeryEasy && data._rarity == EnemyRarity.Epic && _currentEpicEnemyCount >= _maxEpicEnemies)
                continue;

            if (GetSpawnCost(data) > _currentSpawnBudget)
                continue;

            totalWeight += GetRarityWeight(data);
        }

        if (totalWeight <= 0f)
            return null;

        int randomValue = Random.Range(0, totalWeight);

        foreach (EnemyData data in _enemyDataList)
        {
            if (data == null || data._prefab == null)
                continue;

            if (GetSpawnCost(data) > _currentSpawnBudget)
                continue;

            randomValue -= GetRarityWeight(data);

            if (randomValue <= 0f)
                return data;
        }

        return _enemyDataList[_enemyDataList.Length - 1];
    }

    private int GetRarityWeight(EnemyData data)
    {
        int rarityBonus = _elapsedMinutes * _rarityWeightIncreasePerMinute;
        switch (data._rarity)
        {
            case EnemyRarity.Common:
                return _commonWeight + rarityBonus;

            case EnemyRarity.Uncommon:
                return _uncommonWeight + rarityBonus;

            case EnemyRarity.Rare:
                return _rareWeight + rarityBonus;

            case EnemyRarity.Epic:
                return _epicWeight + rarityBonus;

            case EnemyRarity.Legendary:
                return _legendaryWeight + rarityBonus;

            default:
                return 0;
        }
    }

    private int GetSpawnCost(EnemyData data)
    {
        switch (data._rarity)
        {
            case EnemyRarity.Common:
                return 1;

            case EnemyRarity.Uncommon:
                return 2;

            case EnemyRarity.Rare:
                return 4;

            case EnemyRarity.Epic:
                return 7;

            case EnemyRarity.Legendary:
                return 12;

            default:
                return 0;
        }
    }

    private bool SpawnEnemy()
    {
        if (_currentEnemyCount >= _enemyCounts)
            return false;

        EnemyData data = GetRandomEnemyData();

        if (data == null)
            return false;

        int spawnCost = GetSpawnCost(data);

        if (_currentSpawnBudget < spawnCost)
            return false;


        if (!TryGetSpawnPosition(out Vector3 spawnPosition))
            return false;

        GameObject enemyObject = Instantiate(data._prefab, spawnPosition, Quaternion.identity);

        Ennemi enemy = enemyObject.GetComponent<Ennemi>();

        if (enemy != null) 
        {
            enemy.Initialiser(data);
            enemy.SetSpawner(this);

            if (data._rarity == EnemyRarity.Epic)
            {
                _currentEpicEnemyCount++;
            }
        }

        _currentSpawnBudget -= spawnCost;
        _currentEnemyCount++;

        return true;
    }

    public void EnemyDied(EnemyRarity rarity)
    {
        _currentEnemyCount--;

        if (rarity == EnemyRarity.Epic)
        {
            _currentEpicEnemyCount--;
        }

        StartCoroutine(RespawnEnemyCoroutine());
    }

    private IEnumerator RespawnEnemyCoroutine()
    {
        yield return new WaitForSeconds(_respawnDelay);

        while (_currentEnemyCount < _enemyCounts)
        {
            if (SpawnEnemy())
            {
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
