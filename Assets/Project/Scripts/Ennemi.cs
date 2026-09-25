using UnityEngine;
using UnityEngine.AI;

public class Ennemi : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private Transform t_player;
    [SerializeField] private float _speedRotation = 5f;
    private NavMeshAgent agent;
    private Character player;

    [Header("Statistiques")]
    [SerializeField] private int _health;

    [Header("Drop")]
    [SerializeField] private int _rage = 10;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject objetPlayer = GameObject.FindWithTag("Player");

        if (objetPlayer != null)
        {
            player =objetPlayer.GetComponent<Character>();
            t_player = objetPlayer.transform;
        }
    }

    private void Update()
    {
        if (t_player == null)
            return;

        agent.SetDestination(t_player.position);

        TournerVersJoueur();
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
        if (player != null)
        {
            player.AjouterRage(_rage);
        }

        Destroy(gameObject);
    }
}
