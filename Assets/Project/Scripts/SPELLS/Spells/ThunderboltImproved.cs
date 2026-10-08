using UnityEngine;
using System.Collections;

public class ThunderboltImproved : MonoBehaviour, ISpell
{
    private SpellData _data;
    private SpellData _normalSpellData;

    public void Initialiser(SpellCastContext context)
    {
        _data = context._data;
        _normalSpellData = _data._normalSpell;

        transform.position = context._targetPosition;

        StartCoroutine(LancerThunderBolt(context));
    }

    private IEnumerator LancerThunderBolt(SpellCastContext context)
    {
        float elapsedTime = 0f;

        while (elapsedTime < _data._duration)
        {
            Vector2 randomPosition = Random.insideUnitCircle * _data._impactRadius;
            Vector3 spawnPosition = context._targetPosition + new Vector3(randomPosition.x, 0f, randomPosition.y);

            GameObject thunderBoltObject = Instantiate(_data._projectilePrefab, spawnPosition, Quaternion.identity);

            ISpell spell = thunderBoltObject.GetComponent<ISpell>();

            if (spell != null)
            {
                SpellCastContext thunderBoltContext = new SpellCastContext
                {
                    _caster = context._caster,
                    _direction = Vector3.zero,
                    _targetPosition = spawnPosition,
                    _improved = false,
                    _data = _normalSpellData
                };

                spell.Initialiser(thunderBoltContext);
            }
            else
            {
                Destroy(thunderBoltObject);
            }

            yield return new WaitForSeconds(_data._projectileDelay);

            elapsedTime += _data._projectileDelay;
        }

        Destroy(gameObject);
    }
}
