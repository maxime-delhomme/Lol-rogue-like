using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Laser : MonoBehaviour, ISpell
{
    private SpellData _data;
    private Character _caster;

    private float _durationTimer;
    private float _damageTimer;

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;
        _caster = context._caster.GetComponent<Character>();

        transform.position = context._caster.position;

        Vector3 direction = context._direction;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
        {
            Destroy(gameObject);
            return;
        }

        transform.forward = direction.normalized;

        _durationTimer = _data._duration;

        _damageTimer = 0f;

        ConfigurerVisuel();
    }

    private void Update()
    {
        _durationTimer -= Time.deltaTime;
        _damageTimer -= Time.deltaTime;

        if (_damageTimer <= 0f)
        {
            InfligerDegats();
            _damageTimer = _data._damageInterval;
        }

        if (_durationTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void ConfigurerVisuel()
    {
        transform.localScale = new Vector3(_data._width, 1f, _data._range);

        transform.localPosition += transform.forward * (_data._range * 0.5f);
    }

    private void InfligerDegats()
    {
        Vector3 center = transform.position + transform.forward * (_data._range * 0.5f);

        Collider[] colliders = Physics.OverlapBox(center, new Vector3(_data._width * 0.5f, 1f, _data._range * 0.5f), transform.rotation);

        int damage = _caster.CalculateSpellDamage(_data._damage);

        foreach (Collider collider in colliders)
        {
            Ennemi enemy = collider.GetComponentInParent<Ennemi>();

            if (enemy == null)
                continue;

            enemy.TakeDamage(damage);

            _caster.ApplyLifeSteal(damage);
        }
    }
}
