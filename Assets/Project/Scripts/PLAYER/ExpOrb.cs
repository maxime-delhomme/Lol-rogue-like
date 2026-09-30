using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [SerializeField] private int _experience = 10;
    [SerializeField] private float _attractionRange = 4f;
    [SerializeField] private float _attractionSpeed = 3f;
    [SerializeField] private float _maxAttractionSpeed = 12f;
    [SerializeField] private float _attractionAcceleration = 15f;

    private Rigidbody _rigibody;
    private Transform _player;

    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody>();

        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null )
        {
            _player = playerObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (_player == null)
            return;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance > _attractionRange)
            return;

        Vector3 direction = (_player.position - transform.position).normalized;

        float currentSpeed = _rigibody.linearVelocity.magnitude;

        float newSpeed = Mathf.MoveTowards(currentSpeed, _maxAttractionSpeed, _attractionAcceleration * Time.fixedDeltaTime);

        if (newSpeed < _attractionSpeed)
        {
            newSpeed = _attractionSpeed;
        }

        _rigibody.linearVelocity = direction * newSpeed;
    }


    private void OnTriggerEnter(Collider other)
    {
        Character player = other.GetComponent<Character>();

        if (player == null)
        {
            return;
        }

        player.GainExperience(_experience);

        Destroy(gameObject);
    }
}
