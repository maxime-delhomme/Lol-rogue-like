using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Autel : MonoBehaviour, IInteractable
{
    [Header("Reference")]
    [SerializeField] private Outline _outline;

    [Header("Objective")]
    [SerializeField] private int _enemiesToKill = 10;
    [SerializeField] private int _enemiesPerWave = 3;
    [SerializeField] private float _killRadius = 15f;
    [SerializeField] private GameObject _radiusVisual;
    private readonly List<Ennemi> _aliveEnemies = new List<Ennemi>();
    private bool _canCompleteObjective;

    [Header("Spawn")]
    [SerializeField] private EnemyData[] _enemyDataList;
    [SerializeField] private float _waveInterval = 10f;
    [SerializeField] private float _spawnInterval = 0.5f;
    [SerializeField] private float _spawnHeight = 2f;
    [SerializeField] private float _navMeshSampleDistance = 3f;

    private bool _isInteractive = true;
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

    private void Update()
    {
        Debug.Log($"Il reste {_aliveEnemies.Count} ennemi(s) dans l'arène.");

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

    public void Interact()
    {
        if (_isActivated == false && _isCompleted == false)
        {
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
            EnemyData data = _enemyDataList[Random.Range(0, _enemyDataList.Length)];

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

            Debug.Log("Objectif terminé");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _killRadius);
    }
}
