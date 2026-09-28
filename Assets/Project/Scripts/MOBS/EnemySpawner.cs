using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Ennemi")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Ennemi _enemy;

    [Header("Spawn")]
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _enemyCounts = 5;

    [SerializeField] private float _respawnDelay = 2f;
    private int _currentEnemyCount;

    private void Start()
    {
        for (int i = 0; i < _enemyCounts; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];

        GameObject enemyObject = Instantiate(_enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        _enemy = enemyObject.GetComponent<Ennemi>();

        if (_enemy != null) 
        {
            _enemy.SetSpawner(this);
        }

        _currentEnemyCount++;
    }

    public void EnemyDied()
    {
        _currentEnemyCount--;

        Invoke(nameof(RespawnEnemy), _respawnDelay);
    }

    private void RespawnEnemy()
    {
        SpawnEnemy();
    }
}
