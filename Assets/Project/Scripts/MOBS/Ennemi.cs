using UnityEngine;
using UnityEngine.AI;

public class Ennemi : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private Transform t_player;
    [SerializeField] private float _speedRotation = 5f;
    private NavMeshAgent _agent;
    private Character _player;

    private EnemySpawner _spawner;

    [Header("Statistiques")]
    [SerializeField] private int _health;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _attackDelay = 1f;
    private float _attackCooldown = 0f;

    [Header("Drop")]
    [SerializeField] private int _rage = 10;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

        GameObject objetPlayer = GameObject.FindWithTag("Player");

        if (objetPlayer != null)
        {
            _player =objetPlayer.GetComponent<Character>();
            t_player = objetPlayer.transform;
        }
    }

    private void Update()
    {
        if (t_player == null)
            return;

        float distance = Vector3.Distance(transform.position, t_player.position);

        if (distance > _attackRange)
        {
            _agent.SetDestination(t_player.position);
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
            _player.TakeDamage(_damage);
        }

        _attackCooldown = _attackDelay;
    }

    private void TournerVersJoueur()
    {
        Vector3 direction = t_player.position - transform.position;

        direction.y = 0f;

        if(direction.sqrMagnitude >0.01f)
        {
            direction.Normalize();

            Quaternion rotationCible = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotationCible, Time.deltaTime * _speedRotation);
        }
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;

        Debug.Log(gameObject.name + " reçoit " +  damage + " dégats. PV restants : " + _health);

        if(_health <= 0 )
        {
            Death();
        }
    }

    private void Death()
    {
        if (_player != null)
        {
            _player.AjouterRage(_rage);
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
