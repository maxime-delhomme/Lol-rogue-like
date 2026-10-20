using UnityEngine;
using UnityEngine.AI;

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

    private bool _isDead;

    [Header("Effects")]
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private float _damageTextHeight = 2f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    private bool _isAttacking;

    public void Initialiser(EnemyData data)
    {
        _data = data;
        _currentHealth = _data._health;
        _agent.speed = _data._moveSpeed;
    }

    public void SetSpawner(EnemySpawner spawner)
    {
        _spawner = spawner;
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

        if (_isAttacking)
        {
            TournerVersJoueur();
            return;
        }
            

        float distance = Vector3.Distance(transform.position, _playerTransform.position);

        if (distance > _data._attackRange)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_playerTransform.position);
        }
        else
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            Attaquer();
        }

        TournerVersJoueur();
    }

    private void Attaquer()
    {
        if (_isAttacking)
            return;



        _isAttacking = true;
        _agent.isStopped = true;
        _agent.ResetPath();

        if (_animator != null)
            _animator.SetTrigger("Anticipation");

        Invoke(nameof(ExecuterAttaque), _data._attackDelay);
    }

    private void ExecuterAttaque()
    {
        if (_isDead)
            return;

        if (_animator != null)
            _animator.SetTrigger("Attack");
    }

    public void AppliquerAttaque()
    {
        if (_isDead)
            return;

        if (_player != null && _playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position,_playerTransform.position);

            if (distance <= _data._attackRange)
            {
                _player.TakeDamage(_data._damage);
            }
        }


        _isAttacking = false;

        _agent.isStopped = false;
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

        if (_damageTextPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * _damageTextHeight;

            GameObject damageTextObject = Instantiate(_damageTextPrefab, spawnPosition, Quaternion.identity);

            DamageText damageText = damageTextObject.GetComponent<DamageText>();

            if (damageText != null )
            {
                damageText.Initialiser(damage);
            }
        }

        if(_currentHealth <= 0 )
        {
            Death();
        }
    }

    private void Death()
    {
        if (_isDead)
            return;

        _isDead = true;

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

        Autel altar = FindFirstObjectByType<Autel>();

        if (altar != null)
        {
            altar.EnemyKilled(transform.position);
        }

        Destroy(gameObject);
    }
}
