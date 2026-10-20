using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Autel : MonoBehaviour, IInteractable
{

    [Header("Objective")]
    [SerializeField] private int _enemiesToKill = 10;
    [SerializeField] private int _enemiesToSpawn = 10;
    [SerializeField] private float _killRadius = 15f;
    [SerializeField] private GameObject _radiusVisual;

    [Header("Spawn")]
    [SerializeField] private EnemyData[] _enemyDataList;
    [SerializeField] private float _spawnInterval = 0.5f;
    [SerializeField] private float _spawnHeight = 2f;
    [SerializeField] private float _navMeshSampleDistance = 3f;


    private bool _isActivated;
    private bool _isCompleted;
    private int _enemiesKilled;
    private int _enemiesSpawned;

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
    }

    public void Interact()
    {
        if (_isActivated == false && _isCompleted == false)
        {
            _isActivated = true;

            if (_radiusVisual != null)
            {
                _radiusVisual.SetActive(true);
            }

            Debug.Log("Autel activé !");

            StartCoroutine(SpawnEnemiesCoroutine());
        }
            
    }

    private IEnumerator SpawnEnemiesCoroutine()
    {
        int spawnCount = 0;

        while (spawnCount < _enemiesToSpawn && _isCompleted == false)
        {
            if (_enemyDataList.Length > 0)
            {
                EnemyData data = _enemyDataList[UnityEngine.Random.Range(0, _enemyDataList.Length)];

                if (data != null && data._prefab != null)
                {
                    if (TryGetSpawnPosition(out Vector3 spawnPosition))
                    {
                        GameObject enemyObject = Instantiate(data._prefab,spawnPosition,Quaternion.identity);

                        Ennemi enemy = enemyObject.GetComponent<Ennemi>();

                        if (enemy != null)
                        {
                            enemy.Initialiser(data);
                        }

                        spawnCount++;
                        _enemiesSpawned++;
                    }
                }
            }

            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * _killRadius;

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

    public void EnemyKilled(Vector3 enemyPosition)
    {
        if (_isActivated == true && IsCompleted == false)
        {
            float distance = Vector3.Distance(transform.position, enemyPosition);

            if (distance <= _killRadius)
            {
                _enemiesKilled++;
                Debug.Log($"Autel chargé : {_enemiesKilled}/{_enemiesToKill}");

                if (_enemiesKilled >= _enemiesToKill)
                {
                    ObjectivCompleted();
                }
            }
        }
    }

    private void ObjectivCompleted()
    {
        if (_isCompleted == false)
        {
            _isCompleted = true;

            StopAllCoroutines();

            if (_radiusVisual != null)
            {
                _radiusVisual.SetActive(false);
            }

            Debug.Log("Objectif terminé");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _killRadius);
    }
}
