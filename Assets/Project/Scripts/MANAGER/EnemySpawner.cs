using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static EnemyData;

public class EnemySpawner : MonoBehaviour
{
    [Header("Reference")]
    private Character _player;

    [Header("Ennemi")]
    [SerializeField] private EnemyData[] _enemyDataList;
    [SerializeField] private float _commonWeight = 45f;
    [SerializeField] private float _uncommonWeight = 35f;
    [SerializeField] private float _rareWeight = 14f;
    [SerializeField] private float _epicWeight = 5f;
    [SerializeField] private float _legendaryWeight = 1f;

    [Header("Spawn")]
    [SerializeField] private float _minSpawnDistance;
    [SerializeField] private int _maxSpawnAttemps;
    [SerializeField] private int _enemyCounts;

    [SerializeField] private float _respawnDelay;
    private int _currentEnemyCount;

    public void SetPlayer(Character player)
    {
        _player = player;

        StartCoroutine(SpawnEnemiesCoroutine());
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        for (int i = 0; i < _enemyCounts; i++)
        {
            SpawnEnemy();

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

    private bool SpawnEnemy()
    {
        if (_currentEnemyCount >= _enemyCounts)
            return false;

        if (!TryGetSpawnPosition(out Vector3 spawnPosition))
            return false;

        EnemyData data = GetRandomEnemyData();

        if (data == null)
            return false;

        GameObject enemyObject = Instantiate(data._prefab, spawnPosition, Quaternion.identity);

        Ennemi enemy = enemyObject.GetComponent<Ennemi>();

        if (enemy != null) 
        {
            enemy.Initialiser(data);
            enemy.SetSpawner(this);
        }

        _currentEnemyCount++;

        return true;
    }

    public void EnemyDied()
    {
        _currentEnemyCount--;

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
        }

        yield return new WaitForSeconds(0.2f);
    }
}
