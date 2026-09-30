using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class LootSpawner : MonoBehaviour
{
       public static LootSpawner Instance { get; private set; }

    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _spawnRadius;

    private Queue<LootRequest> _lootQueue = new Queue<LootRequest>();
    private bool _isSpawning;

    private struct LootRequest
    {
        public GameObject _prefab;
        public Vector3 _position;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnLoot(GameObject prefab, int amount, Vector3 position)
    {
        if (prefab == null || amount <= 0)
            return;

        for (int i =  0;  i < amount; i++)
        {
            LootRequest request = new LootRequest
            {
                _prefab = prefab,
                _position = position
            };

            _lootQueue.Enqueue(request);
        }

        if (!_isSpawning)
        {
            StartCoroutine(SpawnLootCoroutine());
        }
    }

    private IEnumerator SpawnLootCoroutine()
    {
        _isSpawning = true;

        while (_lootQueue.Count > 0)
        {
            LootRequest request = _lootQueue.Dequeue();

            Vector2 randomOffset = Random.insideUnitCircle * _spawnRadius;

            Vector3 spawnPosition = request._position + new Vector3(randomOffset.x, 0.5f, randomOffset.y);

            Instantiate(request._prefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(_spawnDelay);
        }

        _isSpawning = false;
    }
}
