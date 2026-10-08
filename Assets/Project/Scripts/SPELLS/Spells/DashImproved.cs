using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DashImproved : MonoBehaviour, ISpell
{
    private SpellData _data;
    private NavMeshAgent _agent;
    private Transform _caster;
    private Character _player;

    private Vector3 _targetPosition;
    private Vector3 _direction;

    private readonly HashSet<Ennemi> _hitEnemies = new();

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;
        _caster = context._caster;
        _player = context._caster.GetComponent<Character>();
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
            InfligerDegats(_caster.position, _targetPosition);

            _caster.position = _targetPosition;
            TerminerDash();
            return;
        }

        Vector3 previousPosition = _caster.position;
        Vector3 newPosition = _caster.position + _direction * movementDistance;

        InfligerDegats(previousPosition, newPosition);

        _caster.position += _direction * movementDistance;
    }

    private void InfligerDegats(Vector3 previousPosition, Vector3 newPosition)
    {
        Vector3 direction = newPosition - previousPosition;
        float distance = direction.magnitude;

        if (distance <= 0f)
            return;

        direction.Normalize();

        RaycastHit[] hits = Physics.SphereCastAll(previousPosition, _data._impactRadius, direction, distance);

        int damage = _player.CalculateSpellDamage(_data._damage);

        foreach (RaycastHit hit in hits)
        {
            Ennemi enemy = hit.collider.GetComponentInParent<Ennemi>();

            if (enemy == null)
                continue;

            if (_hitEnemies.Contains(enemy))
                continue;

            _hitEnemies.Add(enemy);
            enemy.TakeDamage(damage);
        }
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
