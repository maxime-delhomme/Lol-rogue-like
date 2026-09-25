using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Ennemi")]
    [SerializeField] private GameObject prefabEnnemi;

    [Header("Spawn")]
    [SerializeField] private Transform[] pointsDeSpawn;
    [SerializeField] private int nombreEnnemis = 5;

    private void Start()
    {
        SpawnEnnemis();
    }

    private void SpawnEnnemis()
    {
        for (int i = 0; i < nombreEnnemis; i++)
        {
            Transform point = pointsDeSpawn[UnityEngine.Random.Range(0, pointsDeSpawn.Length)];

            Instantiate(prefabEnnemi, point.position, point.rotation);
        }
    }
}
