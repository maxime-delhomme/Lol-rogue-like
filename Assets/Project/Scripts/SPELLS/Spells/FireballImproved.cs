using System.Collections;
using UnityEngine;

public class FireballImproved : MonoBehaviour, ISpell
{
    private SpellData _data;

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;

        StartCoroutine(LancerProjectiles(context));
    }

    private IEnumerator LancerProjectiles(SpellCastContext context)
    {
        for (int i = 0; i < _data._projectileCount; i++)
        {
            float angle = 1440f / _data._projectileCount * i;

            Vector3 direction = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            GameObject projectileObject = Instantiate(_data._projectilePrefab, context._caster.position, Quaternion.identity);

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

            yield return new WaitForSeconds(_data._projectileDelay);
        }

        Destroy(gameObject);
    }
}