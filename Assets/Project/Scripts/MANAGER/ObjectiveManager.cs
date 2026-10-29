using System;
using UnityEngine;
using UnityEngine.AI;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Reference")]
    [Header("Objectives")]
    [SerializeField] private GameObject[] _objectivePrefabs;
    [SerializeField] private int _objectiveCount = 3;

    [Header("Spawn")]
    [SerializeField] private float _spawnRadius = 50f;
    [SerializeField] private float _minDistanceBetweenObjectives = 20f;
    [SerializeField] private float _navMeshSampleDistance = 5f;
    [SerializeField] private int _maxSpawnAttempts = 30;

    [SerializeField] private float _altarExtraDistance = 15f;

    private void Start()
    {
        SpawnObjectives();
    }

    private GameObject GetRandomObjectivePrefab()
    {
        if (_objectivePrefabs.Length == 0)
            return null;

        return _objectivePrefabs[UnityEngine.Random.Range(0, _objectivePrefabs.Length)];
    }

    private bool TryGetSpawnPosition(out Vector3 spawnPosition)
    {
        for (int i = 0; i < _maxSpawnAttempts; i++)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * _spawnRadius;

            Vector3 candidatePosition = new Vector3(randomPoint.x, 0f, randomPoint.y);

            if (NavMesh.SamplePosition(candidatePosition, out NavMeshHit hit, _navMeshSampleDistance, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private void SpawnObjectives()
    {
        for (int i = 0; i < _objectiveCount; i++)
        {
            GameObject prefab = GetRandomObjectivePrefab();

            if (prefab == null)
                return;

            bool positionFound = false;

            for (int attempt = 0; attempt < _maxSpawnAttempts; attempt++)
            {
                if (!TryGetSpawnPosition(out Vector3 spawnPosition))
                    continue;

                bool tooClose = false;

                foreach (ObjectiveMarker marker in FindObjectsByType<ObjectiveMarker>())
                {
                    float minDistance = _minDistanceBetweenObjectives;

                    if (prefab.GetComponent<Autel>() != null || marker.GetComponent<Autel>() != null)
                    {
                        minDistance += _altarExtraDistance;
                    }

                    if (Vector3.Distance(marker.transform.position, spawnPosition) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                    continue;

                Instantiate(prefab, spawnPosition, Quaternion.identity);

                positionFound = true;
                break;
            }

            if (!positionFound)
            {
                Debug.LogWarning("Impossible de trouver une position pour un objectif.");
            }
        }
    }
}
