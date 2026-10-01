using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Ennemi : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private Transform _playerTransform;
    private NavMeshAgent _agent;
    private Character _player;
    private EnemySpawner _spawner;

    [Header("Data")]
    [SerializeField] private EnemyData _data;
    private int _currentHealth;
    private float _attackCooldown = 0f;

    public void Initialiser(EnemyData data)
    {
        _data = data;
        _currentHealth = _data._health;
        _agent.speed = _data._moveSpeed;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            _player =playerObject.GetComponent<Character>();
            _playerTransform = playerObject.transform;
        }
    }

    private void Update()
    {
        if (_playerTransform == null)
            return;

        float distance = Vector3.Distance(transform.position, _playerTransform.position);

        if (distance > _data._attackRange)
        {
            _agent.SetDestination(_playerTransform.position);
        }
        else
        {
            _agent.ResetPath();
            Attaquer();
        }

        TournerVersJoueur();
    }

    private void Attaquer()
    {
        _attackCooldown -= Time.deltaTime;

        if (_attackCooldown > 0f)
        {
            return;
        }

        if(_player != null)
        {
            _player.TakeDamage(_data._damage);
        }

        _attackCooldown = _data._attackDelay;
    }

    private void TournerVersJoueur()
    {
        Vector3 direction = _playerTransform.position - transform.position;

        direction.y = 0f;

        if(direction.sqrMagnitude >0.01f)
        {
            direction.Normalize();

            Quaternion rotationCible = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotationCible, Time.deltaTime * _data._speedRotation);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        Debug.Log(gameObject.name + " reçoit " +  damage + " dégats. PV restants : " + _currentHealth);

        if(_currentHealth <= 0 )
        {
            Death();
        }
    }

    private void Death()
    {
        if (_player != null)
        {
            _player.AjouterRage(_data._rage);
        }

        if (LootSpawner.Instance != null)
        {
            LootSpawner.Instance.SpawnLoot(_data._expOrbPrefab, _data._expOrbCount, transform.position);
        }

        if(_spawner != null)
        {
            _spawner.EnemyDied();
        }

        Destroy(gameObject);
    }

    public void SetSpawner(EnemySpawner spawner)
    {
        _spawner = spawner;
    }
}
