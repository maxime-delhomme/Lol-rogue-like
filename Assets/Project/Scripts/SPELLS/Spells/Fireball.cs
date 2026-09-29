using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class Fireball : MonoBehaviour, ISpell
{
    private SpellData _data;
    private Vector3 _direction;
    private Vector3 _startPosition;
    private bool _improved;
    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;
        _direction = context._direction;
        _startPosition = transform.position;
        _improved = context._improved;

        if (_improved)
        {
            VersionAmelioree(context);
        }
    }

    private void VersionAmelioree(SpellCastContext context)
    {
        for (int i = 0; i < _data._projectileCount; i++)
        {
            float angle = 360f / _data._projectileCount * i;

            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            GameObject projectileObject = Instantiate(_data._prefab, context._caster.position, Quaternion.identity);

            ISpell spell = projectileObject.GetComponent<ISpell>();

            if (spell == null)
            {
                Destroy(projectileObject);
                continue;
            }

            SpellCastContext projectileContext = new SpellCastContext
            {
                _caster = context._caster,
                _direction = direction,
                _targetPosition = context._caster.position + direction * _data._range,
                _improved = false,
                _data = _data
            };

            spell.Initialiser(projectileContext);
            
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        transform.position += _direction * _data._speed * Time.deltaTime;

        float distance = Vector3.Distance(_startPosition, transform.position);

        if (distance >= _data._range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ennemi enemy = other.GetComponent<Ennemi>();

        if (enemy == null)
        {
            return;
        }

        Impact();

        Destroy(gameObject);
    }

    private void Impact()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _data._impactRadius);

        foreach (Collider collider in colliders)
        {
            Ennemi enemy = collider.GetComponent<Ennemi>();

            if (enemy == null)
            { 
                continue; 
            }

            int damage = _data._damage;

            if (_improved)
            {
                damage *= 2;
            }

            enemy.TakeDamage(damage);
        }
    }


}
