using UnityEngine;
using UnityEngine.AI;

public class Dash : MonoBehaviour, ISpell
{
    private SpellData _data;
    private NavMeshAgent _agent;
    private Transform _caster;

    private Vector3 _targetPosition;
    private Vector3 _direction;

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;

        _caster = context._caster;

        _agent = context._caster.GetComponent<NavMeshAgent>();

        _targetPosition = context._targetPosition;

        Vector3 direction = _targetPosition - context._caster.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            Destroy(gameObject);
            return;
        }

        _direction = direction.normalized;

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(_targetPosition, out hit, 1f, NavMesh.AllAreas))
        {
            Destroy(gameObject);
            return;
        }

        _targetPosition = hit.position;

        _agent.isStopped = true;
        _agent.updatePosition = false;
    }

    private void Update()
    {
        Vector3 remaining = _targetPosition - _caster.position;
        remaining.y = 0f;

        float distanceRemaining = remaining.magnitude;

        if (distanceRemaining <= 0.05f)
        {
            TerminerDash();
            return;
        }

        float movementDistance = _data._speed * Time.deltaTime;

        if (movementDistance >= distanceRemaining)
        {
            _caster.position = _targetPosition;
            TerminerDash();
            return;
        }

        _caster.position += _direction * movementDistance;
    }

    private void TerminerDash()
    {
        _caster.position = _targetPosition;

        _agent.Warp(_targetPosition);

        _agent.updatePosition = true;
        _agent.isStopped = false;

        Destroy(gameObject);
    }
}
