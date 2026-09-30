using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class Fireball : MonoBehaviour, ISpell
{
    private SpellData _data;
    private Vector3 _direction;
    private Vector3 _startPosition;
    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;
        _direction = context._direction;
        _startPosition = transform.position;
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

            enemy.TakeDamage(damage);
        }
    }


}
