using UnityEngine;

public class WizardSpell : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _maxDistance = 20f;
    private int _damage;
    private bool _hasHit;
    private Vector3 _direction;
    private Vector3 _startPosition;

    public void Initialiser(int damage)
    {
        _damage = damage;
        _direction = transform.forward;
        _startPosition = transform.position;
    }

    private void Update()
    {
        if (_hasHit == false)
        {
            float distanceToTravel = _speed * Time.deltaTime;

            transform.position += _direction * distanceToTravel;

            float distanceTraveled = Vector3.Distance(_startPosition, transform.position);

            if (distanceTraveled >= _maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHit == false && other.CompareTag("Player"))
        {
            Character player = other.GetComponent<Character>();

            if (player != null)
            {
                _hasHit = true;
                player.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
